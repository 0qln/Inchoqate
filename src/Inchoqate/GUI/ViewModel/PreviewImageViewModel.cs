using MvvmHelpers;
using System.Windows.Media;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows;
using OpenTK.Wpf;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Inchoqate.GUI.ViewModel
{
    public class PreviewImageViewModel : BaseViewModel, IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<PreviewImageViewModel>();

        private ShaderModel _shader;
        private VertexArrayModel _vertexArray;
        private readonly HardwareEditQueueModel _hardwareEditQueue; // TODO: replace this 
        private TextureModel? _texture;

        private float[] _vertices =
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
        private Size renderSize;
        private Size sourceSize;

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

            _vertexArray?.Dispose();
            _vertexArray = new VertexArrayModel(_indices, _vertices, BufferUsageHint.StaticDraw);
            _vertexArray.Use();

            _shader = new ShaderModel(
                "./GUI/Shaders/Base.vert",
                "./GUI/Shaders/Base.frag",
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }

            _hardwareEditQueue = new HardwareEditQueueModel(_vertexArray);
        }


        public void RenderToImage(GLWpfControl image)
        {
            //var fb = _hardwareEditQueue.Apply();

            //if (fb is null)
            //{
            //    return;
            //}

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, image.Framebuffer);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //fb.Data.Use(TextureUnit.Texture0);
            _texture.Use(TextureUnit.Texture0);
            _shader.Use();
            _vertexArray.Use();
            GL.DrawElements(PrimitiveType.Triangles, _vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        private float zoom;
        private float panXDelta;
        private float panYDelta;
        private float panXStart;
        private float panYStart;
        private float panSensetivity = 10.0f;
        private float zoomSensetivity = 50.0f;

        public void MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            static float transform(float x, float r, float s, float a = 1.0f)
            {
                var result = -r / (x + r) + a;
                return s * result;
            }

            float zoomDelta = (float)e.Delta > 0 ? .5f : -.5f;


            Point relative = e.GetPosition((IInputElement)sender);
            float relativeXScaled = 0, relativeYScaled = 0;
            if (sender is FrameworkElement frameworkElement)
            {
                relativeXScaled = (float)(relative.X / frameworkElement.ActualWidth) * 2 - 1;
                relativeYScaled = (float)(relative.Y / frameworkElement.ActualHeight) * 2 - 1;
            }

            float newZoom = zoom + (zoomDelta / zoomSensetivity);
            newZoom = Math.Clamp(newZoom, 0, 0.5f);
            if (newZoom == 0.5f) return;

            panXStart = (panXStart + relativeXScaled * zoom) - (relativeXScaled * newZoom);
            panYStart = (panYStart + relativeYScaled * zoom) - (relativeYScaled * newZoom);

            zoom = newZoom;

            Reload();
        }

        public void DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                var relativeXScaled = (float)(e.HorizontalChange / frameworkElement.ActualWidth);
                var relativeYScaled = (float)(e.VerticalChange / frameworkElement.ActualHeight);

                if (zoom != 0)
                {
                    panXDelta = relativeXScaled / zoom / panSensetivity;
                    panYDelta = relativeYScaled / zoom / panSensetivity;
                }
            }

            Reload();
        }

        public void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            panXStart += panXDelta;
            panYStart += panYDelta;
            panXDelta = panYDelta = 0;
        }

        public void DragStarted(object sender, DragStartedEventArgs e)
        {
        }

        public void ResetZoom()
        {
            zoom = 0;
            panXDelta = 0;
            panYDelta = 0;
            panXStart = 0;
            panYStart = 0;

            Reload();
        }

        public void Reload()
        {
            float panX = panXDelta + panXStart;
            float panY = panYDelta + panYStart;

            _vertices =
            [
                // Position             Texture coordinates
                 1.0f,  1.0f, 0.0f,     1.0f - zoom - panX, 1.0f - zoom + panY, // top right
                 1.0f, -1.0f, 0.0f,     1.0f - zoom - panX, 0.0f + zoom + panY, // bottom right
                -1.0f, -1.0f, 0.0f,     0.0f + zoom - panX, 0.0f + zoom + panY, // bottom left
                -1.0f,  1.0f, 0.0f,     0.0f + zoom - panX, 1.0f - zoom + panY  // top left
            ];

            _vertexArray?.Dispose();

            _vertexArray = new VertexArrayModel(_indices, _vertices, BufferUsageHint.StaticDraw);
            _vertexArray.Use();

            _shader?.Dispose();

            _shader = new ShaderModel(
                "./GUI/Shaders/Base.vert",
                "./GUI/Shaders/Base.frag",
                out bool success);
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
