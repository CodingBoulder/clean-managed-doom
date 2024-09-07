using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.ExceptionServices;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using TrippyGL;
using ManagedDoom.Video;

namespace ManagedDoom.Silk
{
    public sealed class SilkVideo : IVideo, IDisposable
    {
        private readonly Renderer _renderer;

        private GraphicsDevice? _device;

        private readonly int _textureWidth;
        private readonly int _textureHeight;

        private readonly byte[] _textureData;
        private Texture2D? _texture;

        private TextureBatcher? _batcher;
        private SimpleShaderProgram? _shader;

        private int _silkWindowWidth;
        private int _silkWindowHeight;

        public SilkVideo(Config config, GameContent content, IWindow window, GL gl)
        {
            try
            {
                Console.Write("Initialize video: ");

                _renderer = new Renderer(config, content);

                _device = new GraphicsDevice(gl);

                if (config.video_highresolution)
                {
                    _textureWidth = 512;
                    _textureHeight = 1024;
                }
                else
                {
                    _textureWidth = 256;
                    _textureHeight = 512;
                }

                _textureData = new byte[4 * _renderer.Width * _renderer.Height];
                _texture = new Texture2D(_device, (uint)_textureWidth, (uint)_textureHeight);
                _texture.SetTextureFilters(TrippyGL.TextureMinFilter.Nearest, TrippyGL.TextureMagFilter.Nearest);

                _batcher = new TextureBatcher(_device);
                _shader = SimpleShaderProgram.Create<VertexColorTexture>(_device);
                _batcher.SetShaderProgram(_shader);

                _device.BlendingEnabled = false;

                Resize(window.Size.X, window.Size.Y);

                Console.WriteLine("OK");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed");
                Dispose();
                ExceptionDispatchInfo.Throw(e);
            }
        }

        public unsafe void Render(Doom doom, Fixed frameFrac)
        {
            _renderer.Render(doom, _textureData, frameFrac);

            _texture.SetData<byte>(_textureData, 0, 0, (uint)_renderer.Height, (uint)_renderer.Width);

            float u = (float)_renderer.Height / _textureWidth;
            float v = (float)_renderer.Width / _textureHeight;
            var tl = new VertexColorTexture(new Vector3(0, 0, 0), Color4b.White, new Vector2(0, 0));
            var tr = new VertexColorTexture(new Vector3(_silkWindowWidth, 0, 0), Color4b.White, new Vector2(0, v));
            var br = new VertexColorTexture(new Vector3(_silkWindowWidth, _silkWindowHeight, 0), Color4b.White, new Vector2(u, v));
            var bl = new VertexColorTexture(new Vector3(0, _silkWindowHeight, 0), Color4b.White, new Vector2(u, 0));

            _batcher.Begin();
            _batcher.DrawRaw(_texture, tl, tr, br, bl);
            _batcher.End();
        }

        public void Resize(int width, int height)
        {
            _silkWindowWidth = width;
            _silkWindowHeight = height;
            _device.SetViewport(0, 0, (uint)width, (uint)height);
            _shader.Projection = Matrix4x4.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
        }

        public void InitializeWipe()
        {
            _renderer.InitializeWipe();
        }

        public bool HasFocus()
        {
            return true;
        }

        public void Dispose()
        {
            Console.WriteLine("Shutdown video.");

            if (_shader != null)
            {
                _shader.Dispose();
                _shader = null;
            }

            if (_batcher != null)
            {
                _batcher.Dispose();
                _batcher = null;
            }

            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }

            if (_device != null)
            {
                _device.Dispose();
                _device = null;
            }
        }

        public int WipeBandCount => _renderer.WipeBandCount;
        public int WipeHeight => _renderer.WipeHeight;

        public int MaxWindowSize => _renderer.MaxWindowSize;

        public int WindowSize
        {
            get => _renderer.WindowSize;
            set => _renderer.WindowSize = value;
        }

        public bool DisplayMessage
        {
            get => _renderer.DisplayMessage;
            set => _renderer.DisplayMessage = value;
        }

        public int MaxGammaCorrectionLevel => _renderer.MaxGammaCorrectionLevel;

        public int GammaCorrectionLevel
        {
            get => _renderer.GammaCorrectionLevel;
            set => _renderer.GammaCorrectionLevel = value;
        }
    }
}
