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
    public sealed class StatusBar
    {
        private readonly World _world;

        // Used for appopriately pained face.
        private int _oldHealth;

        // Used for evil grin.
        private readonly bool[] _oldWeaponsOwned;

        // Count until face changes.
        private int _faceCount;

        // Current face index.
        private int _faceIndex;

        // A random number per tick.
        private int _randomNumber;

        private int _priority;

        private int _lastAttackDown;
        private int _lastPainOffset;

        private readonly DoomRandom _random;

        public StatusBar(World world)
        {
            _world = world;

            _oldHealth = -1;
            _oldWeaponsOwned = new bool[DoomInfo.WeaponInfos.Length];
            Array.Copy(
                world.ConsolePlayer.WeaponOwned,
                _oldWeaponsOwned,
                DoomInfo.WeaponInfos.Length);
            _faceCount = 0;
            _faceIndex = 0;
            _randomNumber = 0;
            _priority = 0;
            _lastAttackDown = -1;
            _lastPainOffset = 0;

            _random = new DoomRandom();
        }

        public void Reset()
        {
            _oldHealth = -1;
            Array.Copy(
                _world.ConsolePlayer.WeaponOwned,
                _oldWeaponsOwned,
                DoomInfo.WeaponInfos.Length);
            _faceCount = 0;
            _faceIndex = 0;
            _randomNumber = 0;
            _priority = 0;
            _lastAttackDown = -1;
            _lastPainOffset = 0;
        }

        public void Update()
        {
            _randomNumber = _random.Next();
            UpdateFace();
        }

        private void UpdateFace()
        {
            Player player = _world.ConsolePlayer;

            if (_priority < 10)
            {
                // Dead.
                if (player.Health == 0)
                {
                    _priority = 9;
                    _faceIndex = Face.DeadIndex;
                    _faceCount = 1;
                }
            }

            if (_priority < 9)
            {
                if (player.BonusCount != 0)
                {
                    // Picking up bonus.
                    bool doEvilGrin = false;

                    for (int i = 0; i < DoomInfo.WeaponInfos.Length; i++)
                    {
                        if (_oldWeaponsOwned[i] != player.WeaponOwned[i])
                        {
                            doEvilGrin = true;
                            _oldWeaponsOwned[i] = player.WeaponOwned[i];
                        }
                    }

                    if (doEvilGrin)
                    {
                        // Evil grin if just picked up weapon.
                        _priority = 8;
                        _faceCount = Face.EvilGrinDuration;
                        _faceIndex = CalcPainOffset() + Face.EvilGrinOffset;
                    }
                }
            }

            if (_priority < 8)
            {
                if (player.DamageCount != 0 &&
                    player.Attacker != null &&
                    player.Attacker != player.Mobj)
                {
                    // Being attacked.
                    _priority = 7;

                    if (player.Health - _oldHealth > Face.MuchPain)
                    {
                        _faceCount = Face.TurnDuration;
                        _faceIndex = CalcPainOffset() + Face.OuchOffset;
                    }
                    else
                    {
                        Angle attackerAngle = Geometry.PointToAngle(
                            player.Mobj.X, player.Mobj.Y,
                            player.Attacker.X, player.Attacker.Y);

                        Angle diff;
                        bool right;
                        if (attackerAngle > player.Mobj.Angle)
                        {
                            // Whether right or left.
                            diff = attackerAngle - player.Mobj.Angle;
                            right = diff > Angle.Ang180;
                        }
                        else
                        {
                            // Whether left or right.
                            diff = player.Mobj.Angle - attackerAngle;
                            right = diff <= Angle.Ang180;
                        }

                        _faceCount = Face.TurnDuration;
                        _faceIndex = CalcPainOffset();

                        if (diff < Angle.Ang45)
                        {
                            // Head-on.
                            _faceIndex += Face.RampageOffset;
                        }
                        else if (right)
                        {
                            // Turn face right.
                            _faceIndex += Face.TurnOffset;
                        }
                        else
                        {
                            // Turn face left.
                            _faceIndex += Face.TurnOffset + 1;
                        }
                    }
                }
            }

            if (_priority < 7)
            {
                // Getting hurt because of your own damn stupidity.
                if (player.DamageCount != 0)
                {
                    if (player.Health - _oldHealth > Face.MuchPain)
                    {
                        _priority = 7;
                        _faceCount = Face.TurnDuration;
                        _faceIndex = CalcPainOffset() + Face.OuchOffset;
                    }
                    else
                    {
                        _priority = 6;
                        _faceCount = Face.TurnDuration;
                        _faceIndex = CalcPainOffset() + Face.RampageOffset;
                    }
                }
            }

            if (_priority < 6)
            {
                // Rapid firing.
                if (player.AttackDown)
                {
                    if (_lastAttackDown == -1)
                    {
                        _lastAttackDown = Face.RampageDelay;
                    }
                    else if (--_lastAttackDown == 0)
                    {
                        _priority = 5;
                        _faceIndex = CalcPainOffset() + Face.RampageOffset;
                        _faceCount = 1;
                        _lastAttackDown = 1;
                    }
                }
                else
                {
                    _lastAttackDown = -1;
                }
            }

            if (_priority < 5)
            {
                // Invulnerability.
                if ((player.Cheats & CheatFlags.GodMode) != 0 ||
                    player.Powers[(int)PowerType.Invulnerability] != 0)
                {
                    _priority = 4;

                    _faceIndex = Face.GodIndex;
                    _faceCount = 1;
                }
            }

            // Look left or look right if the facecount has timed out.
            if (_faceCount == 0)
            {
                _faceIndex = CalcPainOffset() + (_randomNumber % 3);
                _faceCount = Face.StraightFaceDuration;
                _priority = 0;
            }

            _faceCount--;
        }

        private int CalcPainOffset()
        {
            Player player = _world.Options.Players[_world.Options.ConsolePlayer];

            int health = player.Health > 100 ? 100 : player.Health;

            if (health != _oldHealth)
            {
                _lastPainOffset = Face.Stride *
                    (((100 - health) * Face.PainFaceCount) / 101);
                _oldHealth = health;
            }

            return _lastPainOffset;
        }

        public int FaceIndex => _faceIndex;



        public static class Face
        {
            public static readonly int PainFaceCount = 5;
            public static readonly int StraightFaceCount = 3;
            public static readonly int TurnFaceCount = 2;
            public static readonly int SpecialFaceCount = 3;

            public static readonly int Stride = StraightFaceCount + TurnFaceCount + SpecialFaceCount;
            public static readonly int ExtraFaceCount = 2;
            public static readonly int FaceCount = Stride * PainFaceCount + ExtraFaceCount;

            public static readonly int TurnOffset = StraightFaceCount;
            public static readonly int OuchOffset = TurnOffset + TurnFaceCount;
            public static readonly int EvilGrinOffset = OuchOffset + 1;
            public static readonly int RampageOffset = EvilGrinOffset + 1;
            public static readonly int GodIndex = PainFaceCount * Stride;
            public static readonly int DeadIndex = GodIndex + 1;

            public static readonly int EvilGrinDuration = (2 * GameConst.TicRate);
            public static readonly int StraightFaceDuration = (GameConst.TicRate / 2);
            public static readonly int TurnDuration = (1 * GameConst.TicRate);
            public static readonly int OuchDuration = (1 * GameConst.TicRate);
            public static readonly int RampageDelay = (2 * GameConst.TicRate);

            public static readonly int MuchPain = 20;
        }
    }
}
