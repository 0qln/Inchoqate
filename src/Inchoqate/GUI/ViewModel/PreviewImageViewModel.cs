using MvvmHelpers;
using System.Windows.Media;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows;
using OpenTK.Wpf;

namespace Inchoqate.GUI.ViewModel
{
    public class PreviewImageViewModel : BaseViewModel, IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageViewModel>();

        private readonly ShaderModel _shader;
        private readonly VertexArrayModel _vertexArray;
        private readonly HardwareEditQueueModel _hardwareEditQueue; // TODO: replace this 
        private TextureModel? _texture;

        private readonly float[] _vertices =
        [
            // Position         Texture coordinates
             1.0f,  1.0f, 0.0f, 1.0f, 1.0f, // top right
             1.0f, -1.0f, 0.0f, 1.0f, 0.0f, // bottom right
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, // bottom left
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f  // top left
        ];

        private readonly uint[] _indices =
        [
            0, 1, 3,
            1, 2, 3
        ];


        private string? imageSource;
        private Stretch stretch;
        private Size renderSize = Size.Empty;
        private Size sourceSize = Size.Empty;

        public string? ImageSource
        {
            get => imageSource;
            set
            {
                if (value is null) return;

                SetProperty(ref imageSource, value);
                _texture?.Dispose();
                _texture = new TextureModel(imageSource);
                _hardwareEditQueue.SourceTexture = _texture;
                SourceSize = new Size(_texture.Width, _texture.Height);
            }
        }

        public Size SourceSize
        {
            get => sourceSize;
            private set => SetProperty(ref sourceSize, value);
        }

        public Size RenderSize
        {
            get => renderSize;
            set
            {
                SetProperty(ref renderSize, value);
                _hardwareEditQueue.RenderSize = value;
            }
        }


        public PreviewImageViewModel()
        {
            _vertexArray = new VertexArrayModel(_indices, _vertices, BufferUsageHint.StaticDraw);
            _vertexArray.Use();

            _shader = new ShaderModel(
                "./Base.vert",
                "./Base.frag",
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }

            _hardwareEditQueue = new HardwareEditQueueModel(_vertexArray);
        }


        public void RenderToImage(GLWpfControl image)
        {
            var fb = _hardwareEditQueue.Apply();

            if (fb is null)
            {
                return;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, image.Framebuffer);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //fb.Data.Use(TextureUnit.Texture0);
            _texture.Use(TextureUnit.Texture0);
            _shader.Use();
            _vertexArray.Use();
            GL.DrawElements(PrimitiveType.Triangles, _vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _hardwareEditQueue.Dispose();
                _vertexArray.Dispose();
                _shader.Dispose();
                _texture?.Dispose();

                disposedValue = true;
            }
        }

        ~PreviewImageViewModel()
        {
            // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
            // The OpenGL resources have to be released from a thread with an active OpenGL Context.
            // The GC runs on a seperate thread, thus releasing unmanaged GL resources inside the finalizer
            // is not possible.
            if (disposedValue == false)
            {
                _logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
