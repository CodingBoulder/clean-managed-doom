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
using System.Runtime.ExceptionServices;
using DrippyAL;
using ManagedDoom.Audio;

namespace ManagedDoom.Silk
{
    public sealed class SilkSound : ISound, IDisposable
    {
        private static readonly int _channelCount = 8;

        private static readonly float _fastDecay = (float)Math.Pow(0.5, 1.0 / (35 / 5));
        private static readonly float _slowDecay = (float)Math.Pow(0.5, 1.0 / 35);

        private static readonly float _clipDist = 1200;
        private static readonly float _closeDist = 160;
        private static readonly float _attenuator = _clipDist - _closeDist;

        private readonly Config _config;

        private AudioClip?[]? _buffers;
        private readonly float[] _amplitudes;

        private readonly DoomRandom _random;

        private AudioChannel?[]? _channels;
        private readonly ChannelInfo[] _infos;

        private AudioChannel? _uiChannel;
        private Sfx _uiReserved;

        private Mobj? _listener;

        private float _masterVolumeDecay;

        private DateTime _lastUpdate;

        public SilkSound(Config config, GameContent content, AudioDevice device)
        {
            try
            {
                Console.Write("Initialize sound: ");

                _config = config;

                config.audio_soundvolume = Math.Clamp(config.audio_soundvolume, 0, MaxVolume);

                _buffers = new AudioClip[DoomInfo.SfxNames.Length];
                _amplitudes = new float[DoomInfo.SfxNames.Length];

                if (config.audio_randompitch)
                {
                    _random = new DoomRandom();
                }

                for (int i = 0; i < DoomInfo.SfxNames.Length; i++)
                {
                    string name = "DS" + DoomInfo.SfxNames[i].ToString().ToUpper();
                    int lump = content.Wad.GetLumpNumber(name);
                    if (lump == -1)
                    {
                        continue;
                    }

                    Span<byte> samples = GetSamples(content.Wad, name, out int sampleRate, out int sampleCount);
                    if (!samples.IsEmpty)
                    {
                        _buffers[i] = new AudioClip(device, sampleRate, 1, samples);
                        _amplitudes[i] = GetAmplitude(samples, sampleRate, sampleCount);
                    }
                }

                _channels = new AudioChannel[_channelCount];
                _infos = new ChannelInfo[_channelCount];
                for (int i = 0; i < _channels.Length; i++)
                {
                    _channels[i] = new AudioChannel(device);
                    _infos[i] = new ChannelInfo();
                }

                _uiChannel = new AudioChannel(device);
                _uiReserved = Sfx.NONE;

                _masterVolumeDecay = (float)config.audio_soundvolume / MaxVolume;

                _lastUpdate = DateTime.MinValue;

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private static Span<byte> GetSamples(Wad wad, string name, out int sampleRate, out int sampleCount)
        {
            byte[] data = wad.ReadLump(name);

            if (data.Length < 8)
            {
                sampleRate = -1;
                sampleCount = -1;
                return null;
            }

            sampleRate = BitConverter.ToUInt16(data, 2);
            sampleCount = BitConverter.ToInt32(data, 4);

            int offset = 8;
            if (ContainsDmxPadding(data))
            {
                offset += 16;
                sampleCount -= 32;
            }

            if (sampleCount > 0)
            {
                return data.AsSpan(offset, sampleCount);
            }
            else
            {
                return [];
            }
        }

        // Check if the data contains pad bytes.
        // If the first and last 16 samples are the same,
        // the data should contain pad bytes.
        // https://doomwiki.org/wiki/Sound
        private static bool ContainsDmxPadding(byte[] data)
        {
            int sampleCount = BitConverter.ToInt32(data, 4);
            if (sampleCount < 32)
            {
                return false;
            }
            else
            {
                byte first = data[8];
                for (int i = 1; i < 16; i++)
                {
                    if (data[8 + i] != first)
                    {
                        return false;
                    }
                }

                byte last = data[8 + sampleCount - 1];
                for (int i = 1; i < 16; i++)
                {
                    if (data[8 + sampleCount - i - 1] != last)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static float GetAmplitude(Span<byte> samples, int sampleRate, int sampleCount)
        {
            int max = 0;
            if (sampleCount > 0)
            {
                int count = Math.Min(sampleRate / 5, sampleCount);
                for (int t = 0; t < count; t++)
                {
                    int a = samples[t] - 128;
                    if (a < 0)
                    {
                        a = -a;
                    }
                    if (a > max)
                    {
                        max = a;
                    }
                }
            }
            return (float)max / 128;
        }

        public void SetListener(Mobj listener)
        {
            _listener = listener;
        }

        public void Update()
        {
            DateTime now = DateTime.Now;
            if ((now - _lastUpdate).TotalSeconds < 0.01)
            {
                // Don't update so frequently (for timedemo).
                return;
            }

            for (int i = 0; i < _infos.Length; i++)
            {
                ChannelInfo info = _infos[i];
                AudioChannel? channel = _channels[i];

                if (info.Playing != Sfx.NONE)
                {
                    if (channel.State != PlaybackState.Stopped)
                    {
                        if (info.Type == SfxType.Diffuse)
                        {
                            info.Priority *= _slowDecay;
                        }
                        else
                        {
                            info.Priority *= _fastDecay;
                        }
                        SetParam(channel, info);
                    }
                    else
                    {
                        info.Playing = Sfx.NONE;
                        if (info.Reserved == Sfx.NONE)
                        {
                            info.Source = null;
                        }
                    }
                }

                if (info.Reserved != Sfx.NONE)
                {
                    if (info.Playing != Sfx.NONE)
                    {
                        channel.Stop();
                    }

                    channel.AudioClip = _buffers[(int)info.Reserved];
                    SetParam(channel, info);
                    channel.Pitch = GetPitch(info.Type, info.Reserved);
                    channel.Play();
                    info.Playing = info.Reserved;
                    info.Reserved = Sfx.NONE;
                }
            }

            if (_uiReserved != Sfx.NONE)
            {
                if (_uiChannel.State == PlaybackState.Playing)
                {
                    _uiChannel.Stop();
                }
                _uiChannel.Position = new Vector3(0, 0, -1);
                _uiChannel.Volume = _masterVolumeDecay;
                _uiChannel.AudioClip = _buffers[(int)_uiReserved];
                _uiChannel.Play();
                _uiReserved = Sfx.NONE;
            }

            _lastUpdate = now;
        }

        public void StartSound(Sfx sfx)
        {
            if (_buffers[(int)sfx] == null)
            {
                return;
            }

            _uiReserved = sfx;
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type)
        {
            StartSound(mobj, sfx, type, 100);
        }

        public void StartSound(Mobj mobj, Sfx sfx, SfxType type, int volume)
        {
            if (_buffers[(int)sfx] == null)
            {
                return;
            }

            float x = (mobj.X - _listener.X).ToFloat();
            float y = (mobj.Y - _listener.Y).ToFloat();
            float dist = MathF.Sqrt(x * x + y * y);

            float priority;
            if (type == SfxType.Diffuse)
            {
                priority = volume;
            }
            else
            {
                priority = _amplitudes[(int)sfx] * GetDistanceDecay(dist) * volume;
            }

            for (int i = 0; i < _infos.Length; i++)
            {
                ChannelInfo info = _infos[i];
                if (info.Source == mobj && info.Type == type)
                {
                    info.Reserved = sfx;
                    info.Priority = priority;
                    info.Volume = volume;
                    return;
                }
            }

            for (int i = 0; i < _infos.Length; i++)
            {
                ChannelInfo info = _infos[i];
                if (info.Reserved == Sfx.NONE && info.Playing == Sfx.NONE)
                {
                    info.Reserved = sfx;
                    info.Priority = priority;
                    info.Source = mobj;
                    info.Type = type;
                    info.Volume = volume;
                    return;
                }
            }

            float minPriority = float.MaxValue;
            int minChannel = -1;
            for (int i = 0; i < _infos.Length; i++)
            {
                ChannelInfo info = _infos[i];
                if (info.Priority < minPriority)
                {
                    minPriority = info.Priority;
                    minChannel = i;
                }
            }
            if (priority >= minPriority)
            {
                ChannelInfo info = _infos[minChannel];
                info.Reserved = sfx;
                info.Priority = priority;
                info.Source = mobj;
                info.Type = type;
                info.Volume = volume;
            }
        }

        public void StopSound(Mobj mobj)
        {
            for (int i = 0; i < _infos.Length; i++)
            {
                ChannelInfo info = _infos[i];
                if (info.Source == mobj)
                {
                    info.LastX = info.Source.X;
                    info.LastY = info.Source.Y;
                    info.Source = null;
                    info.Volume /= 5;
                }
            }
        }

        public void Reset()
        {
            _random?.Clear();

            for (int i = 0; i < _infos.Length; i++)
            {
                _channels[i].Stop();
                _infos[i].Clear();
            }

            _listener = null;
        }

        public void Pause()
        {
            for (int i = 0; i < _infos.Length; i++)
            {
                AudioChannel? channel = _channels[i];

                if (channel.State == PlaybackState.Playing &&
                    channel.AudioClip.Duration - channel.PlayingOffset > TimeSpan.FromMilliseconds(200))
                {
                    _channels[i].Pause();
                }
            }
        }

        public void Resume()
        {
            for (int i = 0; i < _infos.Length; i++)
            {
                AudioChannel? channel = _channels[i];

                if (channel.State == PlaybackState.Paused)
                {
                    channel.Play();
                }
            }
        }

        private void SetParam(AudioChannel sound, ChannelInfo info)
        {
            if (info.Type == SfxType.Diffuse)
            {
                sound.Position = new Vector3(0, 0, -1);
                sound.Volume = 0.01F * _masterVolumeDecay * info.Volume;
            }
            else
            {
                Fixed sourceX;
                Fixed sourceY;
                if (info.Source == null)
                {
                    sourceX = info.LastX;
                    sourceY = info.LastY;
                }
                else
                {
                    sourceX = info.Source.X;
                    sourceY = info.Source.Y;
                }

                float x = (sourceX - _listener.X).ToFloat();
                float y = (sourceY - _listener.Y).ToFloat();

                if (Math.Abs(x) < 16 && Math.Abs(y) < 16)
                {
                    sound.Position = new Vector3(0, 0, -1);
                    sound.Volume = 0.01F * _masterVolumeDecay * info.Volume;
                }
                else
                {
                    float dist = MathF.Sqrt(x * x + y * y);
                    float angle = MathF.Atan2(y, x) - (float)_listener.Angle.ToRadian();
                    sound.Position = new Vector3(-MathF.Sin(angle), 0, -MathF.Cos(angle));
                    sound.Volume = 0.01F * _masterVolumeDecay * GetDistanceDecay(dist) * info.Volume;
                }
            }
        }

        private static float GetDistanceDecay(float dist)
        {
            if (dist < _closeDist)
            {
                return 1F;
            }
            else
            {
                return Math.Max((_clipDist - dist) / _attenuator, 0F);
            }
        }

        private float GetPitch(SfxType type, Sfx sfx)
        {
            if (_random != null)
            {
                if (sfx == Sfx.ITEMUP || sfx == Sfx.TINK || sfx == Sfx.RADIO)
                {
                    return 1.0F;
                }
                else if (type == SfxType.Voice)
                {
                    return 1.0F + 0.075F * (_random.Next() - 128) / 128;
                }
                else
                {
                    return 1.0F + 0.025F * (_random.Next() - 128) / 128;
                }
            }
            else
            {
                return 1.0F;
            }
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown sound.");

            if (_channels != null)
            {
                for (int i = 0; i < _channels.Length; i++)
                {
                    if (_channels[i] != null)
                    {
                        _channels[i].Stop();
                        _channels[i].Dispose();
                        _channels[i] = null;
                    }
                }
                _channels = null;
            }

            if (_buffers != null)
            {
                for (int i = 0; i < _buffers.Length; i++)
                {
                    if (_buffers[i] != null)
                    {
                        _buffers[i].Dispose();
                        _buffers[i] = null;
                    }
                }
                _buffers = null;
            }

            if (_uiChannel != null)
            {
                _uiChannel.Dispose();
                _uiChannel = null;
            }
        }

        public int MaxVolume
        {
            get
            {
                return 15;
            }
        }

        public int Volume
        {
            get
            {
                return _config.audio_soundvolume;
            }

            set
            {
                _config.audio_soundvolume = value;
                _masterVolumeDecay = (float)_config.audio_soundvolume / MaxVolume;
            }
        }



        private class ChannelInfo
        {
            public Sfx Reserved;
            public Sfx Playing;
            public float Priority;

            public Mobj? Source;
            public SfxType Type;
            public int Volume;
            public Fixed LastX;
            public Fixed LastY;

            public void Clear()
            {
                Reserved = Sfx.NONE;
                Playing = Sfx.NONE;
                Priority = 0;
                Source = null;
                Type = 0;
                Volume = 0;
                LastX = Fixed.Zero;
                LastY = Fixed.Zero;
            }
        }
    }
}
