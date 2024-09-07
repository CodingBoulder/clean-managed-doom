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
using System.Numerics;

namespace ManagedDoom
{
    public sealed class Player
    {
        public static readonly int MaxPlayerCount = 4;

        public static readonly Fixed NormalViewHeight = Fixed.FromInt(41);

        private static readonly string[] _defaultPlayerNames =
        [
            "Green",
            "Indigo",
            "Brown",
            "Red"
        ];

        private readonly int _number;
        private readonly string _name;
        private bool _inGame;

        private Mobj? _mobj;
        private PlayerState _playerState;
        private readonly TicCmd _cmd;

        // Determine POV, including viewpoint bobbing during movement.
        // Focal origin above mobj.Z.
        private Fixed _viewZ;

        // Base height above floor for viewz.
        private Fixed _viewHeight;

        // Bob / squat speed.
        private Fixed _deltaViewHeight;

        // Bounded / scaled total momentum.
        private Fixed _bob;

        // This is only used between levels,
        // mobj.Health is used during levels.
        private int _health;
        private int _armorPoints;

        // Armor type is 0-2.
        private int _armorType;

        // Power ups. invinc and invis are tic counters.
        private readonly int[] _powers;
        private readonly bool[] _cards;
        private bool _backpack;

        // Frags, kills of other players.
        private readonly int[] _frags;

        private WeaponType _readyWeapon;

        // Is WeanponType.NoChange if not changing.
        private WeaponType _pendingWeapon;

        private readonly bool[] _weaponOwned;
        private readonly int[] _ammo;
        private readonly int[] _maxAmmo;

        // True if button down last tic.
        private bool _attackDown;
        private bool _useDown;

        // Bit flags, for cheats and debug.
        private CheatFlags _cheats;

        // Refired shots are less accurate.
        private int _refire;

        // For intermission stats.
        private int _killCount;
        private int _itemCount;
        private int _secretCount;

        // Hint messages.
        private string? _message;
        private int _messageTime;

        // For screen flashing (red or bright).
        private int _damageCount;
        private int _bonusCount;

        // Who did damage (null for floors / ceilings).
        private Mobj? _attacker;

        // So gun flashes light up areas.
        private int _extraLight;

        // Current PLAYPAL, ???
        // can be set to REDCOLORMAP for pain, etc.
        private int _fixedColorMap;

        // Player skin colorshift,
        // 0-3 for which color to draw player.
        private int _colorMap;

        // Overlay view sprites (gun, etc).
        private readonly PlayerSpriteDef[] _playerSprites;

        // True if secret level has been done.
        private bool _didSecret;

        // For frame interpolation.
        private bool _interpolate;
        private Fixed _oldViewZ;
        private Angle _oldAngle;

        public Player(int number)
        {
            _number = number;

            _name = _defaultPlayerNames[number];

            _cmd = new TicCmd();

            _powers = new int[(int)PowerType.Count];
            _cards = new bool[(int)CardType.Count];

            _frags = new int[MaxPlayerCount];

            _weaponOwned = new bool[(int)WeaponType.Count];
            _ammo = new int[(int)AmmoType.Count];
            _maxAmmo = new int[(int)AmmoType.Count];

            _playerSprites = new PlayerSpriteDef[(int)PlayerSprite.Count];
            for (int i = 0; i < _playerSprites.Length; i++)
            {
                _playerSprites[i] = new PlayerSpriteDef();
            }
        }

        public void Clear()
        {
            _mobj = null;
            _playerState = 0;
            _cmd.Clear();

            _viewZ = Fixed.Zero;
            _viewHeight = Fixed.Zero;
            _deltaViewHeight = Fixed.Zero;
            _bob = Fixed.Zero;

            _health = 0;
            _armorPoints = 0;
            _armorType = 0;

            Array.Clear(_powers, 0, _powers.Length);
            Array.Clear(_cards, 0, _cards.Length);
            _backpack = false;

            Array.Clear(_frags, 0, _frags.Length);

            _readyWeapon = 0;
            _pendingWeapon = 0;

            Array.Clear(_weaponOwned, 0, _weaponOwned.Length);
            Array.Clear(_ammo, 0, _ammo.Length);
            Array.Clear(_maxAmmo, 0, _maxAmmo.Length);

            _useDown = false;
            _attackDown = false;

            _cheats = 0;

            _refire = 0;

            _killCount = 0;
            _itemCount = 0;
            _secretCount = 0;

            _message = null;
            _messageTime = 0;

            _damageCount = 0;
            _bonusCount = 0;

            _attacker = null;

            _extraLight = 0;

            _fixedColorMap = 0;

            _colorMap = 0;

            foreach (PlayerSpriteDef psp in _playerSprites)
            {
                psp.Clear();
            }

            _didSecret = false;

            _interpolate = false;
            _oldViewZ = Fixed.Zero;
            _oldAngle = Angle.Ang0;
        }

        public void Reborn()
        {
            _mobj = null;
            _playerState = PlayerState.Live;
            _cmd.Clear();

            _viewZ = Fixed.Zero;
            _viewHeight = Fixed.Zero;
            _deltaViewHeight = Fixed.Zero;
            _bob = Fixed.Zero;

            _health = DoomInfo.DeHackEdConst.InitialHealth;
            _armorPoints = 0;
            _armorType = 0;

            Array.Clear(_powers, 0, _powers.Length);
            Array.Clear(_cards, 0, _cards.Length);
            _backpack = false;

            _readyWeapon = WeaponType.Pistol;
            _pendingWeapon = WeaponType.Pistol;

            Array.Clear(_weaponOwned, 0, _weaponOwned.Length);
            Array.Clear(_ammo, 0, _ammo.Length);
            Array.Clear(_maxAmmo, 0, _maxAmmo.Length);

            _weaponOwned[(int)WeaponType.Fist] = true;
            _weaponOwned[(int)WeaponType.Pistol] = true;
            _ammo[(int)AmmoType.Clip] = DoomInfo.DeHackEdConst.InitialBullets;
            for (int i = 0; i < (int)AmmoType.Count; i++)
            {
                _maxAmmo[i] = DoomInfo.AmmoInfos.Max[i];
            }

            // Don't do anything immediately.
            _useDown = true;
            _attackDown = true;

            _cheats = 0;

            _refire = 0;

            _message = null;
            _messageTime = 0;

            _damageCount = 0;
            _bonusCount = 0;

            _attacker = null;

            _extraLight = 0;

            _fixedColorMap = 0;

            _colorMap = 0;

            foreach (PlayerSpriteDef psp in _playerSprites)
            {
                psp.Clear();
            }

            _didSecret = false;

            _interpolate = false;
            _oldViewZ = Fixed.Zero;
            _oldAngle = Angle.Ang0;
        }

        public void FinishLevel()
        {
            Array.Clear(_powers, 0, _powers.Length);
            Array.Clear(_cards, 0, _cards.Length);

            // Cancel invisibility.
            _mobj.Flags &= ~MobjFlags.Shadow;

            // Cancel gun flashes.
            _extraLight = 0;

            // Cancel ir gogles.
            _fixedColorMap = 0;

            // No palette changes.
            _damageCount = 0;
            _bonusCount = 0;
        }

        public void SendMessage(string message)
        {
            if (ReferenceEquals(_message, (string)DoomInfo.Strings.MSGOFF) &&
                !ReferenceEquals(message, (string)DoomInfo.Strings.MSGON))
            {
                return;
            }

            _message = message;
            _messageTime = 4 * GameConst.TicRate;
        }

        public void UpdateFrameInterpolationInfo()
        {
            _interpolate = true;
            _oldViewZ = _viewZ;
            _oldAngle = _mobj.Angle;
        }

        public void DisableFrameInterpolationForOneFrame()
        {
            _interpolate = false;
        }

        public Fixed GetInterpolatedViewZ(Fixed frameFrac)
        {
            // Without the second condition, flicker will occur on the first frame.
            if (_interpolate && _mobj.World.LevelTime > 1)
            {
                return _oldViewZ + frameFrac * (_viewZ - _oldViewZ);
            }
            else
            {
                return _viewZ;
            }
        }

        public Angle GetInterpolatedAngle(Fixed frameFrac)
        {
            if (_interpolate)
            {
                Angle delta = _mobj.Angle - _oldAngle;
                if (delta < Angle.Ang180)
                {
                    return _oldAngle + Angle.FromDegree(frameFrac.ToDouble() * delta.ToDegree());
                }
                else
                {
                    return _oldAngle - Angle.FromDegree(frameFrac.ToDouble() * (360.0 - delta.ToDegree()));
                }
            }
            else
            {
                return _mobj.Angle;
            }
        }

        public int Number => _number;

        public string Name => _name;

        public bool InGame
        {
            get => _inGame;
            set => _inGame = value;
        }

        public Mobj? Mobj
        {
            get => _mobj;
            set => _mobj = value;
        }

        public PlayerState PlayerState
        {
            get => _playerState;
            set => _playerState = value;
        }

        public TicCmd Cmd
        {
            get => _cmd;
        }

        public Fixed ViewZ
        {
            get => _viewZ;
            set => _viewZ = value;
        }

        public Fixed ViewHeight
        {
            get => _viewHeight;
            set => _viewHeight = value;
        }

        public Fixed DeltaViewHeight
        {
            get => _deltaViewHeight;
            set => _deltaViewHeight = value;
        }

        public Fixed Bob
        {
            get => _bob;
            set => _bob = value;
        }

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public int ArmorPoints
        {
            get => _armorPoints;
            set => _armorPoints = value;
        }

        public int ArmorType
        {
            get => _armorType;
            set => _armorType = value;
        }

        public int[] Powers
        {
            get => _powers;
        }

        public bool[] Cards
        {
            get => _cards;
        }

        public bool Backpack
        {
            get => _backpack;
            set => _backpack = value;
        }

        public int[] Frags
        {
            get => _frags;
        }

        public WeaponType ReadyWeapon
        {
            get => _readyWeapon;
            set => _readyWeapon = value;
        }

        public WeaponType PendingWeapon
        {
            get => _pendingWeapon;
            set => _pendingWeapon = value;
        }

        public bool[] WeaponOwned
        {
            get => _weaponOwned;
        }

        public int[] Ammo
        {
            get => _ammo;
        }

        public int[] MaxAmmo
        {
            get => _maxAmmo;
        }

        public bool AttackDown
        {
            get => _attackDown;
            set => _attackDown = value;
        }

        public bool UseDown
        {
            get => _useDown;
            set => _useDown = value;
        }

        public CheatFlags Cheats
        {
            get => _cheats;
            set => _cheats = value;
        }

        public int Refire
        {
            get => _refire;
            set => _refire = value;
        }

        public int KillCount
        {
            get => _killCount;
            set => _killCount = value;
        }

        public int ItemCount
        {
            get => _itemCount;
            set => _itemCount = value;
        }

        public int SecretCount
        {
            get => _secretCount;
            set => _secretCount = value;
        }

        public string? Message
        {
            get => _message;
            set => _message = value;
        }

        public int MessageTime
        {
            get => _messageTime;
            set => _messageTime = value;
        }

        public int DamageCount
        {
            get => _damageCount;
            set => _damageCount = value;
        }

        public int BonusCount
        {
            get => _bonusCount;
            set => _bonusCount = value;
        }

        public Mobj? Attacker
        {
            get => _attacker;
            set => _attacker = value;
        }

        public int ExtraLight
        {
            get => _extraLight;
            set => _extraLight = value;
        }

        public int FixedColorMap
        {
            get => _fixedColorMap;
            set => _fixedColorMap = value;
        }

        public int ColorMap
        {
            get => _colorMap;
            set => _colorMap = value;
        }

        public PlayerSpriteDef[] PlayerSprites
        {
            get => _playerSprites;
        }

        public bool DidSecret
        {
            get => _didSecret;
            set => _didSecret = value;
        }
    }
}
