using DrippyAL;
using Silk.NET.Input;
using Silk.NET.Input.Glfw;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Silk.NET.Windowing.Glfw;
using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace ManagedDoom.Silk
{
    public partial class SilkDoom : IDisposable
    {
        private readonly CommandLineArgs _args;

        private readonly Config _config;
        private readonly GameContent _content;

        private readonly IWindow _window;

        private GL? _gl;
        private SilkVideo? _video;

        private AudioDevice? _audioDevice;
        private SilkSound? _sound;
        private SilkMusic? _music;

        private SilkUserInput? _userInput;

        private Doom _doom;

        private int _fpsScale;
        private int _frameCount;

        private Exception? _exception;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public SilkDoom(CommandLineArgs args)
        {
            try
            {
                _args = args;

                GlfwWindowing.RegisterPlatform();
                GlfwInput.RegisterPlatform();

                _config = SilkConfigUtilities.GetConfig();

                Wad wad = new(ConfigUtilities.GetWadPaths(args));

                DeHackEd.Initialize(args, wad);

                _content = new GameContent(wad);

                _config.video_screenwidth = Math.Clamp(_config.video_screenwidth, 320, int.MaxValue);
                _config.video_screenheight = Math.Clamp(_config.video_screenheight, 200, int.MaxValue);

                WindowOptions windowOptions = WindowOptions.Default;
                windowOptions.Size = new Vector2D<int>(_config.video_screenwidth, _config.video_screenheight);
                windowOptions.Title = ApplicationInfo.Title;
                windowOptions.VSync = false;
                windowOptions.WindowState = _config.video_fullscreen 
                    ? WindowState.Fullscreen 
                    : WindowState.Normal;

                _window = Window.Create(windowOptions);
                _window.Load += OnLoad;
                _window.Update += OnUpdate;
                _window.Render += OnRender;
                _window.Resize += OnResize;
                _window.Closing += OnClose;
            }
            catch (Exception e)
            {
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        private void Quit()
        {
            if (_exception is not null)
            {
                ExceptionDispatchInfo.Throw(_exception);
            }
        }

        private void OnLoad()
        {
            _gl = _window.CreateOpenGL();
            _gl.ClearColor(0.15F, 0.15F, 0.15F, 1F);
            _gl.Clear(ClearBufferMask.ColorBufferBit);
            _window.SwapBuffers();

            _content
                .InitializeAsync(_cancellationTokenSource.Token)
                .Wait();

            _video = new SilkVideo(_config, _content, _window, _gl);

            if (!_args.nosound.Present && !(_args.nosfx.Present && _args.nomusic.Present))
            {
                _audioDevice = new AudioDevice();
                if (!_args.nosfx.Present)
                {
                    _sound = new SilkSound(_config, _content, _audioDevice);
                }
                if (!_args.nomusic.Present)
                {
                    _music = SilkConfigUtilities.GetMusicInstance(_config, _content, _audioDevice);
                }
            }

            _userInput = new SilkUserInput(_config, _window, this, !_args.nomouse.Present);

            _doom = new Doom(_args, _config, _content, _video, _sound, _music, _userInput);

            _fpsScale = _args.timedemo.Present ? 1 : _config.video_fpsscale;
            _frameCount = -1;
        }

        private void OnUpdate(double obj)
        {
            try
            {
                _frameCount++;

                if (_frameCount % _fpsScale == 0)
                {
                    if (_doom.Update() == UpdateResult.Completed)
                    {
                        _window.Close();
                    }
                }
            }
            catch (Exception e)
            {
                _exception = e;
            }

            if (_exception is not null)
            {
                _window.Close();
            }
        }

        private void OnRender(double obj)
        {
            try
            {
                Fixed frameFrac = Fixed.FromInt(_frameCount % _fpsScale + 1) / _fpsScale;
                _video.Render(_doom, frameFrac);
            }
            catch (Exception e)
            {
                _exception = e;
            }
        }

        private void OnResize(Vector2D<int> obj)
        {
            _video.Resize(obj.X, obj.Y);
        }

        private void OnClose()
        {
            if (_userInput != null)
            {
                _userInput.Dispose();
                _userInput = null;
            }

            if (_music != null)
            {
                _music.Dispose();
                _music = null;
            }

            if (_sound != null)
            {
                _sound.Dispose();
                _sound = null;
            }

            if (_audioDevice != null)
            {
                _audioDevice.Dispose();
                _audioDevice = null;
            }

            if (_video != null)
            {
                _video.Dispose();
                _video = null;
            }

            if (_gl != null)
            {
                _gl.Dispose();
                _gl = null;
            }

            _config.Save(ConfigUtilities.GetConfigPath());
        }

        public void KeyDown(Key key)
        {
            _doom.PostEvent(new DoomEvent(EventType.KeyDown, SilkUserInput.SilkToDoom(key)));
        }

        public void KeyUp(Key key)
        {
            _doom.PostEvent(new DoomEvent(EventType.KeyUp, SilkUserInput.SilkToDoom(key)));
        }

        public void Dispose()
        {
            if (_window != null)
            {
                _window.Close();
                _window.Dispose();
            }

            _cancellationTokenSource.Dispose();

            GC.SuppressFinalize(this);
        }

        public string QuitMessage => _doom.QuitMessage;
    }
}
