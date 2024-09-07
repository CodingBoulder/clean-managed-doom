//
// Copyright (C) 1993-1996 Id Software, Inc.
// Copyright (C) 2019-2020 Nobuaki Tanaka
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//



using System;

namespace ManagedDoom
{
    public class Mobj : Thinker
    {
        //
        // NOTES: mobj_t
        //
        // mobj_ts are used to tell the refresh where to draw an image,
        // tell the world simulation when objects are contacted,
        // and tell the sound driver how to position a sound.
        //
        // The refresh uses the next and prev links to follow
        // lists of things in sectors as they are being drawn.
        // The sprite, frame, and angle elements determine which patch_t
        // is used to draw the sprite if it is visible.
        // The sprite and frame values are allmost allways set
        // from state_t structures.
        // The statescr.exe utility generates the states.h and states.c
        // files that contain the sprite/frame numbers from the
        // statescr.txt source file.
        // The xyz origin point represents a point at the bottom middle
        // of the sprite (between the feet of a biped).
        // This is the default origin position for patch_ts grabbed
        // with lumpy.exe.
        // A walking creature will have its z equal to the floor
        // it is standing on.
        //
        // The sound code uses the x,y, and subsector fields
        // to do stereo positioning of any sound effited by the mobj_t.
        //
        // The play simulation uses the blocklinks, x,y,z, radius, height
        // to determine when mobj_ts are touching each other,
        // touching lines in the map, or hit by trace lines (gunshots,
        // lines of sight, etc).
        // The mobj_t->flags element has various bit flags
        // used by the simulation.
        //
        // Every mobj_t is linked into a single sector
        // based on its origin coordinates.
        // The subsector_t is found with R_PointInSubsector(x,y),
        // and the sector_t can be found with subsector->sector.
        // The sector links are only used by the rendering code,
        // the play simulation does not care about them at all.
        //
        // Any mobj_t that needs to be acted upon by something else
        // in the play world (block movement, be shot, etc) will also
        // need to be linked into the blockmap.
        // If the thing has the MF_NOBLOCK flag set, it will not use
        // the block links. It can still interact with other things,
        // but only as the instigator (missiles will run into other
        // things, but nothing can run into a missile).
        // Each block in the grid is 128*128 units, and knows about
        // every line_t that it contains a piece of, and every
        // interactable mobj_t that has its origin contained.  
        //
        // A valid mobj_t is a mobj_t that has the proper subsector_t
        // filled in for its xy coordinates and is linked into the
        // sector from which the subsector was made, or has the
        // MF_NOSECTOR flag set (the subsector_t needs to be valid
        // even if MF_NOSECTOR is set), and is linked into a blockmap
        // block or has the MF_NOBLOCKMAP flag set.
        // Links should only be modified by the P_[Un]SetThingPosition()
        // functions.
        // Do not change the MF_NO? flags while a thing is valid.
        //
        // Any questions?
        //

        public static readonly Fixed OnFloorZ = Fixed.MinValue;
        public static readonly Fixed OnCeilingZ = Fixed.MaxValue;

        private readonly World _world;

        // Info for drawing: position.
        private Fixed _x;
        private Fixed _y;
        private Fixed _z;

        // More list: links in sector (if needed).
        private Mobj? _sectorNext;
        private Mobj? _sectorPrev;

        // More drawing info: to determine current sprite.
        private Angle _angle; // Orientation.
        private Sprite _sprite; // Used to find patch_t and flip value.
        private int _frame; // Might be ORed with FF_FULLBRIGHT.

        // Interaction info, by BLOCKMAP.
        // Links in blocks (if needed).
        private Mobj? _blockNext;
        private Mobj? _blockPrev;

        private Subsector _subsector;

        // The closest interval over all contacted Sectors.
        private Fixed _floorZ;
        private Fixed _ceilingZ;

        // For movement checking.
        private Fixed _radius;
        private Fixed _height;

        // Momentums, used to update position.
        private Fixed _momX;
        private Fixed _momY;
        private Fixed _momZ;

        // If == validCount, already checked.
        private int _validCount;

        private MobjType _type;
        private MobjInfo _info;

        private int _tics; // State tic counter.
        private MobjStateDef _state;
        private MobjFlags _flags;
        private int _health;

        // Movement direction, movement generation (zig-zagging).
        private Direction _moveDir;
        private int _moveCount; // When 0, select a new dir.

        // Thing being chased / attacked (or null),
        // also the originator for missiles.
        private Mobj? _target;

        // Reaction time: if non 0, don't attack yet.
        // Used by player to freeze a bit after teleporting.
        private int _reactionTime;

        // If >0, the target will be chased
        // no matter what (even if shot).
        private int _threshold;

        // Additional info record for player avatars only.
        // Only valid if type == MT_PLAYER
        private Player? _player;

        // Player number last looked for.
        private int _lastLook;

        // For nightmare respawn.
        private MapThing? _spawnPoint;

        // Thing being chased/attacked for tracers.
        private Mobj _tracer;

        // For frame interpolation.
        private bool _interpolate;
        private Fixed _oldX;
        private Fixed _oldY;
        private Fixed _oldZ;

        public Mobj(World world)
        {
            _world = world;
        }

        public override void Run()
        {
            // Momentum movement.
            if (_momX != Fixed.Zero || _momY != Fixed.Zero ||
                (_flags & MobjFlags.SkullFly) != 0)
            {
                _world.ThingMovement.XYMovement(this);

                if (ThinkerState == ThinkerState.Removed)
                {
                    // Mobj was removed.
                    return;
                }
            }

            if ((_z != _floorZ) || _momZ != Fixed.Zero)
            {
                _world.ThingMovement.ZMovement(this);

                if (ThinkerState == ThinkerState.Removed)
                {
                    // Mobj was removed.
                    return;
                }
            }

            // Cycle through states,
            // calling action functions at transitions.
            if (_tics != -1)
            {
                _tics--;

                // You can cycle through multiple states in a tic.
                if (_tics == 0)
                {
                    if (!SetState(_state.Next))
                    {
                        // Freed itself.
                        return;
                    }
                }
            }
            else
            {
                // Check for nightmare respawn.
                if ((_flags & MobjFlags.CountKill) == 0)
                {
                    return;
                }

                GameOptions options = _world.Options;
                if (!(options.Skill == GameSkill.Nightmare || options.RespawnMonsters))
                {
                    return;
                }

                _moveCount++;

                if (_moveCount < 12 * 35)
                {
                    return;
                }

                if ((_world.LevelTime & 31) != 0)
                {
                    return;
                }

                if (_world.Random.Next() > 4)
                {
                    return;
                }

                NightmareRespawn();
            }
        }

        public bool SetState(MobjState state)
        {
            do
            {
                if (state == MobjState.Null)
                {
                    _state = DoomInfo.States[(int)MobjState.Null];
                    _world.ThingAllocation.RemoveMobj(this);
                    return false;
                }

                MobjStateDef st = DoomInfo.States[(int)state];
                _state = st;
                _tics = GetTics(st);
                _sprite = st.Sprite;
                _frame = st.Frame;

                // Modified handling.
                // Call action functions when the state is set.
                st.MobjAction?.Invoke(_world, this);

                state = st.Next;
            }
            while (_tics == 0);

            return true;
        }

        private int GetTics(MobjStateDef state)
        {
            GameOptions options = _world.Options;
            if (options.FastMonsters || options.Skill == GameSkill.Nightmare)
            {
                if ((int)MobjState.SargRun1 <= state.Number &&
                    state.Number <= (int)MobjState.SargPain2)
                {
                    return state.Tics >> 1;
                }
                else
                {
                    return state.Tics;
                }
            }
            else
            {
                return state.Tics;
            }
        }

        private void NightmareRespawn()
        {
            MapThing sp;
            if (_spawnPoint != null)
            {
                sp = _spawnPoint;
            }
            else
            {
                sp = MapThing.Empty;
            }

            // Somthing is occupying it's position?
            if (!_world.ThingMovement.CheckPosition(this, sp.X, sp.Y))
            {
                // No respwan.
                return;
            }

            ThingAllocation ta = _world.ThingAllocation;

            // Spawn a teleport fog at old spot.
            Mobj fog1 = ta.SpawnMobj(
                _x, _y,
                _subsector.Sector.FloorHeight,
                MobjType.Tfog);

            // Initiate teleport sound.
            _world.StartSound(fog1, Sfx.TELEPT, SfxType.Misc);

            // Spawn a teleport fog at the new spot.
            Subsector ss = Geometry.PointInSubsector(sp.X, sp.Y, _world.Map);

            Mobj fog2 = ta.SpawnMobj(
                sp.X, sp.Y,
                ss.Sector.FloorHeight, MobjType.Tfog);

            _world.StartSound(fog2, Sfx.TELEPT, SfxType.Misc);

            // Spawn the new monster.
            Fixed z;
            if ((_info.Flags & MobjFlags.SpawnCeiling) != 0)
            {
                z = OnCeilingZ;
            }
            else
            {
                z = OnFloorZ;
            }

            // Inherit attributes from deceased one.
            Mobj mobj = ta.SpawnMobj(sp.X, sp.Y, z, _type);
            mobj.SpawnPoint = _spawnPoint;
            mobj.Angle = sp.Angle;

            if ((sp.Flags & ThingFlags.Ambush) != 0)
            {
                mobj.Flags |= MobjFlags.Ambush;
            }

            mobj.ReactionTime = 18;

            // Remove the old monster.
            _world.ThingAllocation.RemoveMobj(this);
        }

        public override void UpdateFrameInterpolationInfo()
        {
            _interpolate = true;
            _oldX = _x;
            _oldY = _y;
            _oldZ = _z;
        }

        public void DisableFrameInterpolationForOneFrame()
        {
            _interpolate = false;
        }

        public Fixed GetInterpolatedX(Fixed frameFrac)
        {
            if (_interpolate)
            {
                return _oldX + frameFrac * (_x - _oldX);
            }
            else
            {
                return _x;
            }
        }

        public Fixed GetInterpolatedY(Fixed frameFrac)
        {
            if (_interpolate)
            {
                return _oldY + frameFrac * (_y - _oldY);
            }
            else
            {
                return _y;
            }
        }

        public Fixed GetInterpolatedZ(Fixed frameFrac)
        {
            if (_interpolate)
            {
                return _oldZ + frameFrac * (_z - _oldZ);
            }
            else
            {
                return _z;
            }
        }

        public World World => _world;

        public Fixed X
        {
            get => _x;
            set => _x = value;
        }

        public Fixed Y
        {
            get => _y;
            set => _y = value;
        }

        public Fixed Z
        {
            get => _z;
            set => _z = value;
        }

        public Mobj? SectorNext
        {
            get => _sectorNext;
            set => _sectorNext = value;
        }

        public Mobj? SectorPrev
        {
            get => _sectorPrev;
            set => _sectorPrev = value;
        }

        public Angle Angle
        {
            get => _angle;
            set => _angle = value;
        }

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public int Frame
        {
            get => _frame;
            set => _frame = value;
        }

        public Mobj? BlockNext
        {
            get => _blockNext;
            set => _blockNext = value;
        }

        public Mobj? BlockPrev
        {
            get => _blockPrev;
            set => _blockPrev = value;
        }

        public Subsector Subsector
        {
            get => _subsector;
            set => _subsector = value;
        }

        public Fixed FloorZ
        {
            get => _floorZ;
            set => _floorZ = value;
        }

        public Fixed CeilingZ
        {
            get => _ceilingZ;
            set => _ceilingZ = value;
        }

        public Fixed Radius
        {
            get => _radius;
            set => _radius = value;
        }

        public Fixed Height
        {
            get => _height;
            set => _height = value;
        }

        public Fixed MomX
        {
            get => _momX;
            set => _momX = value;
        }

        public Fixed MomY
        {
            get => _momY;
            set => _momY = value;
        }

        public Fixed MomZ
        {
            get => _momZ;
            set => _momZ = value;
        }

        public int ValidCount
        {
            get => _validCount;
            set => _validCount = value;
        }

        public MobjType Type
        {
            get => _type;
            set => _type = value;
        }

        public MobjInfo Info
        {
            get => _info;
            set => _info = value;
        }

        public int Tics
        {
            get => _tics;
            set => _tics = value;
        }

        public MobjStateDef State
        {
            get => _state;
            set => _state = value;
        }

        public MobjFlags Flags
        {
            get => _flags;
            set => _flags = value;
        }

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public Direction MoveDir
        {
            get => _moveDir;
            set => _moveDir = value;
        }

        public int MoveCount
        {
            get => _moveCount;
            set => _moveCount = value;
        }

        public Mobj? Target
        {
            get => _target;
            set => _target = value;
        }

        public int ReactionTime
        {
            get => _reactionTime;
            set => _reactionTime = value;
        }

        public int Threshold
        {
            get => _threshold;
            set => _threshold = value;
        }

        public Player? Player
        {
            get => _player;
            set => _player = value;
        }

        public int LastLook
        {
            get => _lastLook;
            set => _lastLook = value;
        }

        public MapThing? SpawnPoint
        {
            get => _spawnPoint;
            set => _spawnPoint = value;
        }

        public Mobj Tracer
        {
            get => _tracer;
            set => _tracer = value;
        }
    }
}
