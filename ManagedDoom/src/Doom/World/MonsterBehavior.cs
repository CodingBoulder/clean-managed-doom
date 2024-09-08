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
    public sealed class MonsterBehavior
    {
        private readonly World _world;

        public MonsterBehavior(World world)
        {
            _world = world;

            InitVile();
            InitBossDeath();
            InitBrain();
        }



        ////////////////////////////////////////////////////////////
        // Sleeping monster
        ////////////////////////////////////////////////////////////

        private bool LookForPlayers(Mobj actor, bool allAround)
        {
            Player[] players = _world.Options.Players;

            int count = 0;
            int stop = (actor.LastLook - 1) & 3;

            for (; ; actor.LastLook = (actor.LastLook + 1) & 3)
            {
                if (!players[actor.LastLook].InGame)
                {
                    continue;
                }

                if (count++ == 2 || actor.LastLook == stop)
                {
                    // Done looking.
                    return false;
                }

                Player player = players[actor.LastLook];

                if (player.Health <= 0)
                {
                    // Player is dead.
                    continue;
                }

                if (!_world.VisibilityCheck.CheckSight(actor, player.Mobj))
                {
                    // Out of sight.
                    continue;
                }

                if (!allAround)
                {
                    Angle angle = Geometry.PointToAngle(
                        actor.X, actor.Y,
                        player.Mobj.X, player.Mobj.Y) - actor.Angle;

                    if (angle > Angle.Ang90 && angle < Angle.Ang270)
                    {
                        Fixed dist = Geometry.AproxDistance(
                            player.Mobj.X - actor.X,
                            player.Mobj.Y - actor.Y);

                        // If real close, react anyway.
                        if (dist > WeaponBehavior.MeleeRange)
                        {
                            // Behind back.
                            continue;
                        }
                    }
                }

                actor.Target = player.Mobj;

                return true;
            }
        }


        public void Look(Mobj actor)
        {
            // Any shot will wake up.
            actor.Threshold = 0;

            Mobj? target = actor.Subsector.Sector.SoundTarget;

            if (target != null && (target.Flags & MobjFlags.Shootable) != 0)
            {
                actor.Target = target;

                if ((actor.Flags & MobjFlags.Ambush) != 0)
                {
                    if (_world.VisibilityCheck.CheckSight(actor, actor.Target))
                    {
                        goto seeYou;
                    }
                }
                else
                {
                    goto seeYou;
                }
            }

            if (!LookForPlayers(actor, false))
            {
                return;
            }

        // Go into chase state.
        seeYou:
            if (actor.Info.SeeSound != 0)
            {
                int sound;

                switch (actor.Info.SeeSound)
                {
                    case Sfx.POSIT1:
                    case Sfx.POSIT2:
                    case Sfx.POSIT3:
                        sound = (int)Sfx.POSIT1 + _world.Random.Next() % 3;
                        break;

                    case Sfx.BGSIT1:
                    case Sfx.BGSIT2:
                        sound = (int)Sfx.BGSIT1 + _world.Random.Next() % 2;
                        break;

                    default:
                        sound = (int)actor.Info.SeeSound;
                        break;
                }

                if (actor.Type == MobjType.Spider || actor.Type == MobjType.Cyborg)
                {
                    // Full volume for boss monsters.
                    _world.StartSound(actor, (Sfx)sound, SfxType.Diffuse);
                }
                else
                {
                    _world.StartSound(actor, (Sfx)sound, SfxType.Voice);
                }
            }

            actor.SetState(actor.Info.SeeState);
        }



        ////////////////////////////////////////////////////////////
        // Monster AI
        ////////////////////////////////////////////////////////////

        private static readonly Fixed[] _xSpeed =
        [
            new Fixed(Fixed.FracUnit),
            new Fixed(47000),
            new Fixed(0),
            new Fixed(-47000),
            new Fixed(-Fixed.FracUnit),
            new Fixed(-47000),
            new Fixed(0),
            new Fixed(47000)
        ];

        private static readonly Fixed[] _ySpeed =
        [
            new Fixed(0),
            new Fixed(47000),
            new Fixed(Fixed.FracUnit),
            new Fixed(47000),
            new Fixed(0),
            new Fixed(-47000),
            new Fixed(-Fixed.FracUnit),
            new Fixed(-47000)
        ];

        private bool Move(Mobj actor)
        {
            if (actor.MoveDir == Direction.None)
            {
                return false;
            }

            if ((int)actor.MoveDir >= 8)
            {
                throw new Exception("Weird actor->movedir!");
            }

            Fixed tryX = actor.X + actor.Info.Speed * _xSpeed[(int)actor.MoveDir];
            Fixed tryY = actor.Y + actor.Info.Speed * _ySpeed[(int)actor.MoveDir];

            ThingMovement tm = _world.ThingMovement;

            bool tryOk = tm.TryMove(actor, tryX, tryY);

            if (!tryOk)
            {
                // Open any specials.
                if ((actor.Flags & MobjFlags.Float) != 0 && tm.FloatOk)
                {
                    // Must adjust height.
                    if (actor.Z < tm.CurrentFloorZ)
                    {
                        actor.Z += ThingMovement.FloatSpeed;
                    }
                    else
                    {
                        actor.Z -= ThingMovement.FloatSpeed;
                    }

                    actor.Flags |= MobjFlags.InFloat;

                    return true;
                }

                if (tm.crossedSpecialCount == 0)
                {
                    return false;
                }

                actor.MoveDir = Direction.None;
                bool good = false;
                while (tm.crossedSpecialCount-- > 0)
                {
                    LineDef line = tm.crossedSpecials[tm.crossedSpecialCount];
                    // If the special is not a door that can be opened,
                    // return false.
                    if (_world.MapInteraction.UseSpecialLine(actor, line, 0))
                    {
                        good = true;
                    }
                }
                return good;
            }
            else
            {
                actor.Flags &= ~MobjFlags.InFloat;
            }

            if ((actor.Flags & MobjFlags.Float) == 0)
            {
                actor.Z = actor.FloorZ;
            }

            return true;
        }


        private bool TryWalk(Mobj actor)
        {
            if (!Move(actor))
            {
                return false;
            }

            actor.MoveCount = _world.Random.Next() & 15;

            return true;
        }


        private static readonly Direction[] _opposite =
        [
            Direction.west,
            Direction.Southwest,
            Direction.South,
            Direction.Southeast,
            Direction.East,
            Direction.Northeast,
            Direction.North,
            Direction.Northwest,
            Direction.None
        ];

        private static readonly Direction[] _diags =
        [
            Direction.Northwest,
            Direction.Northeast,
            Direction.Southwest,
            Direction.Southeast
        ];

        private readonly Direction[] _choices = new Direction[3];

        private void NewChaseDir(Mobj actor)
        {
            if (actor.Target == null)
            {
                throw new Exception("Called with no target.");
            }

            Direction oldDir = actor.MoveDir;
            Direction turnAround = _opposite[(int)oldDir];

            Fixed deltaX = actor.Target.X - actor.X;
            Fixed deltaY = actor.Target.Y - actor.Y;

            if (deltaX > Fixed.FromInt(10))
            {
                _choices[1] = Direction.East;
            }
            else if (deltaX < Fixed.FromInt(-10))
            {
                _choices[1] = Direction.west;
            }
            else
            {
                _choices[1] = Direction.None;
            }

            if (deltaY < Fixed.FromInt(-10))
            {
                _choices[2] = Direction.South;
            }
            else if (deltaY > Fixed.FromInt(10))
            {
                _choices[2] = Direction.North;
            }
            else
            {
                _choices[2] = Direction.None;
            }

            // Try direct route.
            if (_choices[1] != Direction.None && _choices[2] != Direction.None)
            {
                int a = (deltaY < Fixed.Zero) ? 1 : 0;
                int b = (deltaX > Fixed.Zero) ? 1 : 0;
                actor.MoveDir = _diags[(a << 1) + b];

                if (actor.MoveDir != turnAround && TryWalk(actor))
                {
                    return;
                }
            }

            // Try other directions.
            if (_world.Random.Next() > 200 || Fixed.Abs(deltaY) > Fixed.Abs(deltaX))
            {
                Direction temp = _choices[1];
                _choices[1] = _choices[2];
                _choices[2] = temp;
            }

            if (_choices[1] == turnAround)
            {
                _choices[1] = Direction.None;
            }

            if (_choices[2] == turnAround)
            {
                _choices[2] = Direction.None;
            }

            if (_choices[1] != Direction.None)
            {
                actor.MoveDir = _choices[1];

                if (TryWalk(actor))
                {
                    // Either moved forward or attacked.
                    return;
                }
            }

            if (_choices[2] != Direction.None)
            {
                actor.MoveDir = _choices[2];

                if (TryWalk(actor))
                {
                    return;
                }
            }

            // There is no direct path to the player, so pick another direction.
            if (oldDir != Direction.None)
            {
                actor.MoveDir = oldDir;

                if (TryWalk(actor))
                {
                    return;
                }
            }

            // Randomly determine direction of search.
            if ((_world.Random.Next() & 1) != 0)
            {
                for (int dir = (int)Direction.East; dir <= (int)Direction.Southeast; dir++)
                {
                    if ((Direction)dir != turnAround)
                    {
                        actor.MoveDir = (Direction)dir;

                        if (TryWalk(actor))
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                for (int dir = (int)Direction.Southeast; dir != ((int)Direction.East - 1); dir--)
                {
                    if ((Direction)dir != turnAround)
                    {
                        actor.MoveDir = (Direction)dir;

                        if (TryWalk(actor))
                        {
                            return;
                        }
                    }
                }
            }

            if (turnAround != Direction.None)
            {
                actor.MoveDir = turnAround;

                if (TryWalk(actor))
                {
                    return;
                }
            }

            // Can not move.
            actor.MoveDir = Direction.None;
        }


        private bool CheckMeleeRange(Mobj actor)
        {
            if (actor.Target == null)
            {
                return false;
            }

            Mobj target = actor.Target;

            Fixed dist = Geometry.AproxDistance(target.X - actor.X, target.Y - actor.Y);

            if (dist >= WeaponBehavior.MeleeRange - Fixed.FromInt(20) + target.Info.Radius)
            {
                return false;
            }

            if (!_world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                return false;
            }

            return true;
        }


        private bool CheckMissileRange(Mobj actor)
        {
            if (!_world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                return false;
            }

            if ((actor.Flags & MobjFlags.JustHit) != 0)
            {
                // The target just hit the enemy, so fight back!
                actor.Flags &= ~MobjFlags.JustHit;

                return true;
            }

            if (actor.ReactionTime > 0)
            {
                // Do not attack yet
                return false;
            }

            // OPTIMIZE:
            //     Get this from a global checksight.
            Fixed dist = Geometry.AproxDistance(
                actor.X - actor.Target.X,
                actor.Y - actor.Target.Y) - Fixed.FromInt(64);

            if (actor.Info.MeleeState == 0)
            {
                // No melee attack, so fire more.
                dist -= Fixed.FromInt(128);
            }

            int attackDist = dist.Data >> 16;

            if (actor.Type == MobjType.Vile)
            {
                if (attackDist > 14 * 64)
                {
                    // Too far away.
                    return false;
                }
            }

            if (actor.Type == MobjType.Undead)
            {
                if (attackDist < 196)
                {
                    // Close for fist attack.
                    return false;
                }

                attackDist >>= 1;
            }


            if (actor.Type == MobjType.Cyborg ||
                actor.Type == MobjType.Spider ||
                actor.Type == MobjType.Skull)
            {
                attackDist >>= 1;
            }

            if (attackDist > 200)
            {
                attackDist = 200;
            }

            if (actor.Type == MobjType.Cyborg && attackDist > 160)
            {
                attackDist = 160;
            }

            if (_world.Random.Next() < attackDist)
            {
                return false;
            }

            return true;
        }


        public void Chase(Mobj actor)
        {
            if (actor.ReactionTime > 0)
            {
                actor.ReactionTime--;
            }

            // Modify target threshold.
            if (actor.Threshold > 0)
            {
                if (actor.Target == null || actor.Target.Health <= 0)
                {
                    actor.Threshold = 0;
                }
                else
                {
                    actor.Threshold--;
                }
            }

            // Turn towards movement direction if not there yet.
            if ((int)actor.MoveDir < 8)
            {
                actor.Angle = new Angle((int)actor.Angle.Data & (7 << 29));

                int delta = (int)(actor.Angle - new Angle((int)actor.MoveDir << 29)).Data;

                if (delta > 0)
                {
                    actor.Angle -= new Angle(Angle.Ang90.Data / 2);
                }
                else if (delta < 0)
                {
                    actor.Angle += new Angle(Angle.Ang90.Data / 2);
                }
            }

            if (actor.Target == null || (actor.Target.Flags & MobjFlags.Shootable) == 0)
            {
                // Look for a new target.
                if (LookForPlayers(actor, true))
                {
                    // Got a new target.
                    return;
                }

                actor.SetState(actor.Info.SpawnState);

                return;
            }

            // Do not attack twice in a row.
            if ((actor.Flags & MobjFlags.JustAttacked) != 0)
            {
                actor.Flags &= ~MobjFlags.JustAttacked;

                if (_world.Options.Skill != GameSkill.Nightmare &&
                    !_world.Options.FastMonsters)
                {
                    NewChaseDir(actor);
                }

                return;
            }

            // Check for melee attack.
            if (actor.Info.MeleeState != 0 && CheckMeleeRange(actor))
            {
                if (actor.Info.AttackSound != 0)
                {
                    _world.StartSound(actor, actor.Info.AttackSound, SfxType.Weapon);
                }

                actor.SetState(actor.Info.MeleeState);

                return;
            }

            // Check for missile attack.
            if (actor.Info.MissileState != 0)
            {
                if (_world.Options.Skill < GameSkill.Nightmare &&
                    !_world.Options.FastMonsters &&
                    actor.MoveCount != 0)
                {
                    goto noMissile;
                }

                if (!CheckMissileRange(actor))
                {
                    goto noMissile;
                }

                actor.SetState(actor.Info.MissileState);
                actor.Flags |= MobjFlags.JustAttacked;

                return;
            }

        noMissile:
            // Possibly choose another target.
            if (_world.Options.NetGame &&
                actor.Threshold == 0 &&
                !_world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                if (LookForPlayers(actor, true))
                {
                    // Got a new target.
                    return;
                }
            }

            // Chase towards player.
            if (--actor.MoveCount < 0 || !Move(actor))
            {
                NewChaseDir(actor);
            }

            // Make active sound.
            if (actor.Info.ActiveSound != 0 && _world.Random.Next() < 3)
            {
                _world.StartSound(actor, actor.Info.ActiveSound, SfxType.Voice);
            }
        }



        ////////////////////////////////////////////////////////////
        // Monster death
        ////////////////////////////////////////////////////////////

        public void Pain(Mobj actor)
        {
            if (actor.Info.PainSound != 0)
            {
                _world.StartSound(actor, actor.Info.PainSound, SfxType.Voice);
            }
        }

        public void Scream(Mobj actor)
        {
            int sound;

            switch (actor.Info.DeathSound)
            {
                case 0:
                    return;

                case Sfx.PODTH1:
                case Sfx.PODTH2:
                case Sfx.PODTH3:
                    sound = (int)Sfx.PODTH1 + _world.Random.Next() % 3;
                    break;

                case Sfx.BGDTH1:
                case Sfx.BGDTH2:
                    sound = (int)Sfx.BGDTH1 + _world.Random.Next() % 2;
                    break;

                default:
                    sound = (int)actor.Info.DeathSound;
                    break;
            }

            // Check for bosses.
            if (actor.Type == MobjType.Spider || actor.Type == MobjType.Cyborg)
            {
                // Full volume.
                _world.StartSound(actor, (Sfx)sound, SfxType.Diffuse);
            }
            else
            {
                _world.StartSound(actor, (Sfx)sound, SfxType.Voice);
            }
        }

        public void XScream(Mobj actor)
        {
            _world.StartSound(actor, Sfx.SLOP, SfxType.Voice);
        }

        public void Fall(Mobj actor)
        {
            // Actor is on ground, it can be walked over.
            actor.Flags &= ~MobjFlags.Solid;
        }



        ////////////////////////////////////////////////////////////
        // Monster attack
        ////////////////////////////////////////////////////////////

        public void FaceTarget(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            actor.Flags &= ~MobjFlags.Ambush;

            actor.Angle = Geometry.PointToAngle(
                actor.X, actor.Y,
                actor.Target.X, actor.Target.Y);

            DoomRandom random = _world.Random;

            if ((actor.Target.Flags & MobjFlags.Shadow) != 0)
            {
                actor.Angle += new Angle((random.Next() - random.Next()) << 21);
            }
        }


        public void PosAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            Angle angle = actor.Angle;
            Fixed slope = _world.Hitscan.AimLineAttack(actor, angle, WeaponBehavior.MissileRange);

            _world.StartSound(actor, Sfx.PISTOL, SfxType.Weapon);

            DoomRandom random = _world.Random;
            angle += new Angle((random.Next() - random.Next()) << 20);
            int damage = ((random.Next() % 5) + 1) * 3;

            _world.Hitscan.LineAttack(actor, angle, WeaponBehavior.MissileRange, slope, damage);
        }


        public void SPosAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            _world.StartSound(actor, Sfx.SHOTGN, SfxType.Weapon);

            FaceTarget(actor);

            Angle center = actor.Angle;
            Fixed slope = _world.Hitscan.AimLineAttack(actor, center, WeaponBehavior.MissileRange);

            DoomRandom random = _world.Random;

            for (int i = 0; i < 3; i++)
            {
                Angle angle = center + new Angle((random.Next() - random.Next()) << 20);
                int damage = ((random.Next() % 5) + 1) * 3;

                _world.Hitscan.LineAttack(actor, angle, WeaponBehavior.MissileRange, slope, damage);
            }
        }


        public void CPosAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            _world.StartSound(actor, Sfx.SHOTGN, SfxType.Weapon);

            FaceTarget(actor);

            Angle center = actor.Angle;
            Fixed slope = _world.Hitscan.AimLineAttack(actor, center, WeaponBehavior.MissileRange);

            DoomRandom random = _world.Random;
            Angle angle = center + new Angle((random.Next() - random.Next()) << 20);
            int damage = ((random.Next() % 5) + 1) * 3;

            _world.Hitscan.LineAttack(actor, angle, WeaponBehavior.MissileRange, slope, damage);
        }


        public void CPosRefire(Mobj actor)
        {
            // Keep firing unless target got out of sight.
            FaceTarget(actor);

            if (_world.Random.Next() < 40)
            {
                return;
            }

            if (actor.Target == null ||
                actor.Target.Health <= 0 ||
                !_world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                actor.SetState(actor.Info.SeeState);
            }
        }


        public void TroopAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            if (CheckMeleeRange(actor))
            {
                _world.StartSound(actor, Sfx.CLAW, SfxType.Weapon);

                int damage = (_world.Random.Next() % 8 + 1) * 3;
                _world.ThingInteraction.DamageMobj(actor.Target, actor, actor, damage);

                return;
            }

            // Launch a missile.
            _world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Troopshot);
        }


        public void SargAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            if (CheckMeleeRange(actor))
            {
                int damage = ((_world.Random.Next() % 10) + 1) * 4;
                _world.ThingInteraction.DamageMobj(actor.Target, actor, actor, damage);
            }
        }


        public void HeadAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            if (CheckMeleeRange(actor))
            {
                int damage = (_world.Random.Next() % 6 + 1) * 10;
                _world.ThingInteraction.DamageMobj(actor.Target, actor, actor, damage);

                return;
            }

            // Launch a missile.
            _world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Headshot);
        }


        public void BruisAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            if (CheckMeleeRange(actor))
            {
                _world.StartSound(actor, Sfx.CLAW, SfxType.Weapon);

                int damage = (_world.Random.Next() % 8 + 1) * 10;
                _world.ThingInteraction.DamageMobj(actor.Target, actor, actor, damage);

                return;
            }

            // Launch a missile.
            _world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Bruisershot);
        }


        private static readonly Fixed _skullSpeed = Fixed.FromInt(20);

        public void SkullAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            Mobj dest = actor.Target;

            actor.Flags |= MobjFlags.SkullFly;

            _world.StartSound(actor, actor.Info.AttackSound, SfxType.Voice);

            FaceTarget(actor);

            Angle angle = actor.Angle;
            actor.MomX = _skullSpeed * Trig.Cos(angle);
            actor.MomY = _skullSpeed * Trig.Sin(angle);

            Fixed dist = Geometry.AproxDistance(dest.X - actor.X, dest.Y - actor.Y);

            int num = (dest.Z + (dest.Height >> 1) - actor.Z).Data;
            int den = dist.Data / _skullSpeed.Data;
            if (den < 1)
            {
                den = 1;
            }

            actor.MomZ = new Fixed(num / den);
        }


        public void FatRaise(Mobj actor)
        {
            FaceTarget(actor);

            _world.StartSound(actor, Sfx.MANATK, SfxType.Voice);
        }


        private static readonly Angle _fatSpread = Angle.Ang90 / 8;

        public void FatAttack1(Mobj actor)
        {
            FaceTarget(actor);

            ThingAllocation ta = _world.ThingAllocation;

            // Change direction to...
            actor.Angle += _fatSpread;
            Mobj target = _world.SubstNullMobj(actor.Target);
            ta.SpawnMissile(actor, target, MobjType.Fatshot);

            Mobj missile = ta.SpawnMissile(actor, target, MobjType.Fatshot);
            missile.Angle += _fatSpread;
            Angle angle = missile.Angle;
            missile.MomX = new Fixed(missile.Info.Speed) * Trig.Cos(angle);
            missile.MomY = new Fixed(missile.Info.Speed) * Trig.Sin(angle);
        }

        public void FatAttack2(Mobj actor)
        {
            FaceTarget(actor);

            ThingAllocation ta = _world.ThingAllocation;

            // Now here choose opposite deviation.
            actor.Angle -= _fatSpread;
            Mobj target = _world.SubstNullMobj(actor.Target);
            ta.SpawnMissile(actor, target, MobjType.Fatshot);

            Mobj missile = ta.SpawnMissile(actor, target, MobjType.Fatshot);
            missile.Angle -= _fatSpread * 2;
            Angle angle = missile.Angle;
            missile.MomX = new Fixed(missile.Info.Speed) * Trig.Cos(angle);
            missile.MomY = new Fixed(missile.Info.Speed) * Trig.Sin(angle);
        }

        public void FatAttack3(Mobj actor)
        {
            FaceTarget(actor);

            ThingAllocation ta = _world.ThingAllocation;

            Mobj target = _world.SubstNullMobj(actor.Target);

            Mobj missile1 = ta.SpawnMissile(actor, target, MobjType.Fatshot);
            missile1.Angle -= _fatSpread / 2;
            Angle angle1 = missile1.Angle;
            missile1.MomX = new Fixed(missile1.Info.Speed) * Trig.Cos(angle1);
            missile1.MomY = new Fixed(missile1.Info.Speed) * Trig.Sin(angle1);

            Mobj missile2 = ta.SpawnMissile(actor, target, MobjType.Fatshot);
            missile2.Angle += _fatSpread / 2;
            Angle angle2 = missile2.Angle;
            missile2.MomX = new Fixed(missile2.Info.Speed) * Trig.Cos(angle2);
            missile2.MomY = new Fixed(missile2.Info.Speed) * Trig.Sin(angle2);
        }


        public void BspiAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            // Launch a missile.
            _world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Arachplaz);
        }


        public void SpidRefire(Mobj actor)
        {
            // Keep firing unless target got out of sight.
            FaceTarget(actor);

            if (_world.Random.Next() < 10)
            {
                return;
            }

            if (actor.Target == null ||
                actor.Target.Health <= 0 ||
                !_world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                actor.SetState(actor.Info.SeeState);
            }
        }


        public void CyberAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            _world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Rocket);
        }



        ////////////////////////////////////////////////////////////
        // Miscellaneous
        ////////////////////////////////////////////////////////////

        public void Explode(Mobj actor)
        {
            _world.ThingInteraction.RadiusAttack(actor, actor.Target, 128);
        }


        public void Metal(Mobj actor)
        {
            _world.StartSound(actor, Sfx.METAL, SfxType.Footstep);

            Chase(actor);
        }


        public void BabyMetal(Mobj actor)
        {
            _world.StartSound(actor, Sfx.BSPWLK, SfxType.Footstep);

            Chase(actor);
        }


        public void Hoof(Mobj actor)
        {
            _world.StartSound(actor, Sfx.HOOF, SfxType.Footstep);

            Chase(actor);
        }



        ////////////////////////////////////////////////////////////
        // Arch vile
        ////////////////////////////////////////////////////////////

        private Func<Mobj, bool> _vileCheckFunc;
        private Mobj _vileTargetCorpse;
        private Fixed _vileTryX;
        private Fixed _vileTryY;

        private void InitVile()
        {
            _vileCheckFunc = VileCheck;
        }


        private bool VileCheck(Mobj thing)
        {
            if ((thing.Flags & MobjFlags.Corpse) == 0)
            {
                // Not a monster.
                return true;
            }

            if (thing.Tics != -1)
            {
                // Not lying still yet.
                return true;
            }

            if (thing.Info.Raisestate == MobjState.Null)
            {
                // Monster doesn't have a raise state.
                return true;
            }

            Fixed maxDist = thing.Info.Radius + DoomInfo.MobjInfos[(int)MobjType.Vile].Radius;

            if (Fixed.Abs(thing.X - _vileTryX) > maxDist ||
                Fixed.Abs(thing.Y - _vileTryY) > maxDist)
            {
                // Not actually touching.
                return true;
            }

            _vileTargetCorpse = thing;
            _vileTargetCorpse.MomX = _vileTargetCorpse.MomY = Fixed.Zero;
            _vileTargetCorpse.Height <<= 2;

            bool check = _world.ThingMovement.CheckPosition(
                _vileTargetCorpse,
                _vileTargetCorpse.X,
                _vileTargetCorpse.Y);

            _vileTargetCorpse.Height >>= 2;

            if (!check)
            {
                // Doesn't fit here.
                return true;
            }

            // Got one, so stop checking.
            return false;
        }


        public void VileChase(Mobj actor)
        {
            if (actor.MoveDir != Direction.None)
            {
                // Check for corpses to raise.
                _vileTryX = actor.X + actor.Info.Speed * _xSpeed[(int)actor.MoveDir];
                _vileTryY = actor.Y + actor.Info.Speed * _ySpeed[(int)actor.MoveDir];

                BlockMap bm = _world.Map.BlockMap;

                Fixed maxRadius = GameConst.MaxThingRadius * 2;
                int blockX1 = bm.GetBlockX(_vileTryX - maxRadius);
                int blockX2 = bm.GetBlockX(_vileTryX + maxRadius);
                int blockY1 = bm.GetBlockY(_vileTryY - maxRadius);
                int blockY2 = bm.GetBlockY(_vileTryY + maxRadius);

                for (int bx = blockX1; bx <= blockX2; bx++)
                {
                    for (int by = blockY1; by <= blockY2; by++)
                    {
                        // Call VileCheck to check whether object is a corpse that canbe raised.
                        if (!bm.IterateThings(bx, by, _vileCheckFunc))
                        {
                            // Got one!
                            Mobj? temp = actor.Target;
                            actor.Target = _vileTargetCorpse;
                            FaceTarget(actor);
                            actor.Target = temp;
                            actor.SetState(MobjState.VileHeal1);

                            _world.StartSound(_vileTargetCorpse, Sfx.SLOP, SfxType.Misc);

                            MobjInfo info = _vileTargetCorpse.Info;
                            _vileTargetCorpse.SetState(info.Raisestate);
                            _vileTargetCorpse.Height <<= 2;
                            _vileTargetCorpse.Flags = info.Flags;
                            _vileTargetCorpse.Health = info.SpawnHealth;
                            _vileTargetCorpse.Target = null;

                            return;
                        }
                    }
                }
            }

            // Return to normal attack.
            Chase(actor);
        }


        public void VileStart(Mobj actor)
        {
            _world.StartSound(actor, Sfx.VILATK, SfxType.Weapon);
        }


        public void StartFire(Mobj actor)
        {
            _world.StartSound(actor, Sfx.FLAMST, SfxType.Weapon);

            Fire(actor);
        }


        public void FireCrackle(Mobj actor)
        {
            _world.StartSound(actor, Sfx.FLAME, SfxType.Weapon);

            Fire(actor);
        }


        public void Fire(Mobj actor)
        {
            Mobj dest = actor.Tracer;

            if (dest == null)
            {
                return;
            }

            Mobj target = _world.SubstNullMobj(actor.Target);

            // Don't move it if the vile lost sight.
            if (!_world.VisibilityCheck.CheckSight(target, dest))
            {
                return;
            }

            _world.ThingMovement.UnsetThingPosition(actor);

            Angle angle = dest.Angle;
            actor.X = dest.X + Fixed.FromInt(24) * Trig.Cos(angle);
            actor.Y = dest.Y + Fixed.FromInt(24) * Trig.Sin(angle);
            actor.Z = dest.Z;

            _world.ThingMovement.SetThingPosition(actor);
        }


        public void VileTarget(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            Mobj fog = _world.ThingAllocation.SpawnMobj(
                actor.Target.X,
                actor.Target.X,
                actor.Target.Z,
                MobjType.Fire);

            actor.Tracer = fog;
            fog.Target = actor;
            fog.Tracer = actor.Target;
            Fire(fog);
        }


        public void VileAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            if (!_world.VisibilityCheck.CheckSight(actor, actor.Target))
            {
                return;
            }

            _world.StartSound(actor, Sfx.BAREXP, SfxType.Weapon);
            _world.ThingInteraction.DamageMobj(actor.Target, actor, actor, 20);
            actor.Target.MomZ = Fixed.FromInt(1000) / actor.Target.Info.Mass;

            Mobj fire = actor.Tracer;
            if (fire == null)
            {
                return;
            }

            Angle angle = actor.Angle;

            // Move the fire between the vile and the player.
            fire.X = actor.Target.X - Fixed.FromInt(24) * Trig.Cos(angle);
            fire.Y = actor.Target.Y - Fixed.FromInt(24) * Trig.Sin(angle);
            _world.ThingInteraction.RadiusAttack(fire, actor, 70);
        }



        ////////////////////////////////////////////////////////////
        // Revenant
        ////////////////////////////////////////////////////////////

        public void SkelMissile(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            // Missile spawns higher.
            actor.Z += Fixed.FromInt(16);

            Mobj missile = _world.ThingAllocation.SpawnMissile(actor, actor.Target, MobjType.Tracer);

            // Back to normal.
            actor.Z -= Fixed.FromInt(16);

            missile.X += missile.MomX;
            missile.Y += missile.MomY;
            missile.Tracer = actor.Target;
        }


        private static readonly Angle _traceAngle = new(0xc000000);

        public void Tracer(Mobj actor)
        {
            if ((_world.GameTic & 3) != 0)
            {
                return;
            }

            // Spawn a puff of smoke behind the rocket.
            _world.Hitscan.SpawnPuff(actor.X, actor.Y, actor.Z);

            Mobj smoke = _world.ThingAllocation.SpawnMobj(
                actor.X - actor.MomX,
                actor.Y - actor.MomY,
                actor.Z,
                MobjType.Smoke);

            smoke.MomZ = Fixed.One;
            smoke.Tics -= _world.Random.Next() & 3;
            if (smoke.Tics < 1)
            {
                smoke.Tics = 1;
            }

            // Adjust direction.
            Mobj dest = actor.Tracer;

            if (dest == null || dest.Health <= 0)
            {
                return;
            }

            // Change angle.
            Angle exact = Geometry.PointToAngle(
                actor.X, actor.Y,
                dest.X, dest.Y);

            if (exact != actor.Angle)
            {
                if (exact - actor.Angle > Angle.Ang180)
                {
                    actor.Angle -= _traceAngle;
                    if (exact - actor.Angle < Angle.Ang180)
                    {
                        actor.Angle = exact;
                    }
                }
                else
                {
                    actor.Angle += _traceAngle;
                    if (exact - actor.Angle > Angle.Ang180)
                    {
                        actor.Angle = exact;
                    }
                }
            }

            exact = actor.Angle;
            actor.MomX = new Fixed(actor.Info.Speed) * Trig.Cos(exact);
            actor.MomY = new Fixed(actor.Info.Speed) * Trig.Sin(exact);

            // Change slope.
            Fixed dist = Geometry.AproxDistance(
                dest.X - actor.X,
                dest.Y - actor.Y);

            int num = (dest.Z + Fixed.FromInt(40) - actor.Z).Data;
            int den = dist.Data / actor.Info.Speed;
            if (den < 1)
            {
                den = 1;
            }

            var slope = new Fixed(num / den);

            if (slope < actor.MomZ)
            {
                actor.MomZ -= Fixed.One / 8;
            }
            else
            {
                actor.MomZ += Fixed.One / 8;
            }
        }


        public void SkelWhoosh(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            _world.StartSound(actor, Sfx.SKESWG, SfxType.Weapon);
        }


        public void SkelFist(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            if (CheckMeleeRange(actor))
            {
                int damage = ((_world.Random.Next() % 10) + 1) * 6;
                _world.StartSound(actor, Sfx.SKEPCH, SfxType.Weapon);
                _world.ThingInteraction.DamageMobj(actor.Target, actor, actor, damage);
            }
        }



        ////////////////////////////////////////////////////////////
        // Pain elemental
        ////////////////////////////////////////////////////////////

        private void PainShootSkull(Mobj actor, Angle angle)
        {
            // Count total number of skull currently on the level.
            int count = 0;

            foreach (Thinker thinker in _world.Thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj != null && mobj.Type == MobjType.Skull)
                {
                    count++;
                }
            }

            // If there are allready 20 skulls on the level,
            // don't spit another one.
            if (count > 20)
            {
                return;
            }

            // Okay, there's playe for another one.

            Fixed preStep = Fixed.FromInt(4) +
                3 * (actor.Info.Radius + DoomInfo.MobjInfos[(int)MobjType.Skull].Radius) / 2;

            Fixed x = actor.X + preStep * Trig.Cos(angle);
            Fixed y = actor.Y + preStep * Trig.Sin(angle);
            Fixed z = actor.Z + Fixed.FromInt(8);

            Mobj skull = _world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Skull);

            // Check for movements.
            if (!_world.ThingMovement.TryMove(skull, skull.X, skull.Y))
            {
                // Kill it immediately.
                _world.ThingInteraction.DamageMobj(skull, actor, actor, 10000);
                return;
            }

            skull.Target = actor.Target;

            SkullAttack(skull);
        }


        public void PainAttack(Mobj actor)
        {
            if (actor.Target == null)
            {
                return;
            }

            FaceTarget(actor);

            PainShootSkull(actor, actor.Angle);
        }


        public void PainDie(Mobj actor)
        {
            Fall(actor);

            PainShootSkull(actor, actor.Angle + Angle.Ang90);
            PainShootSkull(actor, actor.Angle + Angle.Ang180);
            PainShootSkull(actor, actor.Angle + Angle.Ang270);
        }



        ////////////////////////////////////////////////////////////
        // Boss death
        ////////////////////////////////////////////////////////////

        private LineDef _junk;

        private void InitBossDeath()
        {
            var v = new Vertex(Fixed.Zero, Fixed.Zero);
            _junk = new LineDef(v, v, 0, 0, 0, null, null);
        }


        public void BossDeath(Mobj actor)
        {
            GameOptions options = _world.Options;
            if (options.GameMode == GameMode.Commercial)
            {
                if (options.Map != 7)
                {
                    return;
                }

                if ((actor.Type != MobjType.Fatso) && (actor.Type != MobjType.Baby))
                {
                    return;
                }
            }
            else
            {
                switch (options.Episode)
                {
                    case 1:
                        if (options.Map != 8)
                        {
                            return;
                        }

                        if (actor.Type != MobjType.Bruiser)
                        {
                            return;
                        }

                        break;

                    case 2:
                        if (options.Map != 8)
                        {
                            return;
                        }

                        if (actor.Type != MobjType.Cyborg)
                        {
                            return;
                        }

                        break;

                    case 3:
                        if (options.Map != 8)
                        {
                            return;
                        }

                        if (actor.Type != MobjType.Spider)
                        {
                            return;
                        }

                        break;

                    case 4:
                        switch (options.Map)
                        {
                            case 6:
                                if (actor.Type != MobjType.Cyborg)
                                {
                                    return;
                                }

                                break;

                            case 8:
                                if (actor.Type != MobjType.Spider)
                                {
                                    return;
                                }

                                break;

                            default:
                                return;
                        }
                        break;

                    default:
                        if (options.Map != 8)
                        {
                            return;
                        }

                        break;
                }
            }

            // Make sure there is a player alive for victory.
            Player[] players = _world.Options.Players;
            int i;
            for (i = 0; i < Player.MaxPlayerCount; i++)
            {
                if (players[i].InGame && players[i].Health > 0)
                {
                    break;
                }
            }

            if (i == Player.MaxPlayerCount)
            {
                // No one left alive, so do not end game.
                return;
            }

            // Scan the remaining thinkers to see if all bosses are dead.
            foreach (Thinker thinker in _world.Thinkers)
            {
                var mo2 = thinker as Mobj;
                if (mo2 == null)
                {
                    continue;
                }

                if (mo2 != actor && mo2.Type == actor.Type && mo2.Health > 0)
                {
                    // Other boss not dead.
                    return;
                }
            }

            // Victory!
            if (options.GameMode == GameMode.Commercial)
            {
                if (options.Map == 7)
                {
                    if (actor.Type == MobjType.Fatso)
                    {
                        _junk.Tag = 666;
                        _world.SectorAction.DoFloor(_junk, FloorMoveType.LowerFloorToLowest);
                        return;
                    }

                    if (actor.Type == MobjType.Baby)
                    {
                        _junk.Tag = 667;
                        _world.SectorAction.DoFloor(_junk, FloorMoveType.RaiseToTexture);
                        return;
                    }
                }
            }
            else
            {
                switch (options.Episode)
                {
                    case 1:
                        _junk.Tag = 666;
                        _world.SectorAction.DoFloor(_junk, FloorMoveType.LowerFloorToLowest);
                        return;

                    case 4:
                        switch (options.Map)
                        {
                            case 6:
                                _junk.Tag = 666;
                                _world.SectorAction.DoDoor(_junk, VerticalDoorType.BlazeOpen);
                                return;

                            case 8:
                                _junk.Tag = 666;
                                _world.SectorAction.DoFloor(_junk, FloorMoveType.LowerFloorToLowest);
                                return;
                        }
                        break;
                }
            }

            _world.ExitLevel();
        }


        public void KeenDie(Mobj actor)
        {
            Fall(actor);

            // scan the remaining thinkers
            // to see if all Keens are dead
            foreach (Thinker thinker in _world.Thinkers)
            {
                var mo2 = thinker as Mobj;
                if (mo2 == null)
                {
                    continue;
                }

                if (mo2 != actor && mo2.Type == actor.Type && mo2.Health > 0)
                {
                    // other Keen not dead
                    return;
                }
            }

            _junk.Tag = 666;
            _world.SectorAction.DoDoor(_junk, VerticalDoorType.Open);
        }



        ////////////////////////////////////////////////////////////
        // Icon of sin
        ////////////////////////////////////////////////////////////

        private Mobj[] _brainTargets;
        private int _brainTargetCount;
        private int _currentBrainTarget;
        private bool _easy;

        private void InitBrain()
        {
            _brainTargets = new Mobj[32];
            _brainTargetCount = 0;
            _currentBrainTarget = 0;
            _easy = false;
        }


        public void BrainAwake(Mobj actor)
        {
            // Find all the target spots.
            _brainTargetCount = 0;
            _currentBrainTarget = 0;

            foreach (Thinker thinker in _world.Thinkers)
            {
                var mobj = thinker as Mobj;
                if (mobj == null)
                {
                    // Not a mobj.
                    continue;
                }

                if (mobj.Type == MobjType.Bosstarget)
                {
                    _brainTargets[_brainTargetCount] = mobj;
                    _brainTargetCount++;
                }
            }

            _world.StartSound(actor, Sfx.BOSSIT, SfxType.Diffuse);
        }


        public void BrainPain(Mobj actor)
        {
            _world.StartSound(actor, Sfx.BOSPN, SfxType.Diffuse);
        }


        public void BrainScream(Mobj actor)
        {
            DoomRandom random = _world.Random;

            for (Fixed x = actor.X - Fixed.FromInt(196); x < actor.X + Fixed.FromInt(320); x += Fixed.FromInt(8))
            {
                Fixed y = actor.Y - Fixed.FromInt(320);
                Fixed z = new Fixed(128) + random.Next() * Fixed.FromInt(2);

                Mobj explosion = _world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Rocket);
                explosion.MomZ = new Fixed(random.Next() * 512);
                explosion.SetState(MobjState.Brainexplode1);
                explosion.Tics -= random.Next() & 7;
                if (explosion.Tics < 1)
                {
                    explosion.Tics = 1;
                }
            }

            _world.StartSound(actor, Sfx.BOSDTH, SfxType.Diffuse);
        }


        public void BrainExplode(Mobj actor)
        {
            DoomRandom random = _world.Random;

            Fixed x = actor.X + new Fixed((random.Next() - random.Next()) * 2048);
            Fixed y = actor.Y;
            Fixed z = new Fixed(128) + random.Next() * Fixed.FromInt(2);

            Mobj explosion = _world.ThingAllocation.SpawnMobj(x, y, z, MobjType.Rocket);
            explosion.MomZ = new Fixed(random.Next() * 512);
            explosion.SetState(MobjState.Brainexplode1);
            explosion.Tics -= random.Next() & 7;
            if (explosion.Tics < 1)
            {
                explosion.Tics = 1;
            }
        }


        public void BrainDie()
        {
            _world.ExitLevel();
        }


        public void BrainSpit(Mobj actor)
        {
            _easy = !_easy;
            if (_world.Options.Skill <= GameSkill.Easy && (!_easy))
            {
                return;
            }

            // If the game is reconstructed from a savedata, brain targets might be cleared.
            // If so, re-initialize them to avoid crash.
            if (_brainTargetCount == 0)
            {
                BrainAwake(actor);
            }

            // Shoot a cube at current target.
            Mobj target = _brainTargets[_currentBrainTarget];
            _currentBrainTarget = (_currentBrainTarget + 1) % _brainTargetCount;

            // Spawn brain missile.
            Mobj missile = _world.ThingAllocation.SpawnMissile(actor, target, MobjType.Spawnshot);
            missile.Target = target;
            missile.ReactionTime = ((target.Y - actor.Y).Data / missile.MomY.Data) / missile.State.Tics;

            _world.StartSound(actor, Sfx.BOSPIT, SfxType.Diffuse);
        }


        public void SpawnSound(Mobj actor)
        {
            _world.StartSound(actor, Sfx.BOSCUB, SfxType.Misc);
            SpawnFly(actor);
        }


        public void SpawnFly(Mobj actor)
        {
            if (--actor.ReactionTime > 0)
            {
                // Still flying.
                return;
            }

            Mobj? target = actor.Target;

            // If the game is reconstructed from a savedata, the target might be null.
            // If so, use own position to spawn the monster.
            if (target == null)
            {
                target = actor;
                actor.Z = actor.Subsector.Sector.FloorHeight;
            }

            ThingAllocation ta = _world.ThingAllocation;

            // First spawn teleport fog.
            Mobj fog = ta.SpawnMobj(target.X, target.Y, target.Z, MobjType.Spawnfire);
            _world.StartSound(fog, Sfx.TELEPT, SfxType.Misc);

            // Randomly select monster to spawn.
            int r = _world.Random.Next();

            // Probability distribution (kind of :), decreasing likelihood.
            MobjType type;
            if (r < 50)
            {
                type = MobjType.Troop;
            }
            else if (r < 90)
            {
                type = MobjType.Sergeant;
            }
            else if (r < 120)
            {
                type = MobjType.Shadows;
            }
            else if (r < 130)
            {
                type = MobjType.Pain;
            }
            else if (r < 160)
            {
                type = MobjType.Head;
            }
            else if (r < 162)
            {
                type = MobjType.Vile;
            }
            else if (r < 172)
            {
                type = MobjType.Undead;
            }
            else if (r < 192)
            {
                type = MobjType.Baby;
            }
            else if (r < 222)
            {
                type = MobjType.Fatso;
            }
            else if (r < 246)
            {
                type = MobjType.Knight;
            }
            else
            {
                type = MobjType.Bruiser;
            }

            Mobj monster = ta.SpawnMobj(target.X, target.Y, target.Z, type);
            if (LookForPlayers(monster, true))
            {
                monster.SetState(monster.Info.SeeState);
            }

            // Telefrag anything in this spot.
            _world.ThingMovement.TeleportMove(monster, monster.X, monster.Y);

            // Remove self (i.e., cube).
            _world.ThingAllocation.RemoveMobj(actor);
        }
    }
}
