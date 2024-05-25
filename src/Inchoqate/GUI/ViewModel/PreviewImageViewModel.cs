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

        private readonly ShaderModel _shader;
        private readonly VertexArrayModel _vertexArray;
        private readonly GpuEditQueueModel _editQueue; // TODO: replace this 
        private TextureModel? _texture;

        private float[] _vertices =
        [
            // Position             Texture coordinates
             1.0f,  1.0f, 0.0f,     1.0f, 1.0f, // top right
             1.0f, -1.0f, 0.0f,     1.0f, 0.0f, // bottom right
            -1.0f, -1.0f, 0.0f,     0.0f, 0.0f, // bottom left
            -1.0f,  1.0f, 0.0f,     0.0f, 1.0f  // top left
        ];

        private readonly uint[] _indices =
        [
            0, 1, 3,
            1, 2, 3
        ];


        private string? imageSource;
        private Size renderSize;
        private Size sourceSize;
        private Size boundsSize;

        public string? ImageSource
        {
            get => imageSource;
            set
            {
                if (value is null) return;

                SetProperty(ref imageSource, value);
                _texture?.Dispose();
                _texture = new TextureModel(imageSource);
                _editQueue.SourceTexture = _texture;
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
                _editQueue.RenderSize = value;
            }
        }

        public Size BoundsSize
        {
            get => boundsSize;
            set
            {
                SetProperty(ref boundsSize, value);
                Reload();
            }
        }

        private float _panXDelta;
        private float _panYDelta;
        private float _panXStart;
        private float _panYStart;
        private float panSensitivity = 1.0f;
        private float zoomLevels = 40.0f;
        private float zoom;

        public float PanSensitivity
        {
            get => panSensitivity;
            set => SetProperty(ref panSensitivity, value);
        }

        public float ZoomLevels
        {
            get => zoomLevels;
            set => SetProperty(ref zoomLevels, value);
        }

        /// <summary>
        /// Range: [0;0.5)
        /// </summary>
        public float Zoom
        {
            get => zoom;
            set
            {
                if (value < 0 || value >= 0.5)
                {
                    throw new ArgumentException(nameof(value));
                }

                SetProperty(ref zoom, value);
                Reload();
            }
        }


        public PreviewImageViewModel()
        {
            _vertexArray = new VertexArrayModel(_indices, _vertices, BufferUsageHint.StaticDraw);
            _vertexArray.Use();

            _vertexArray?.Dispose();
            _vertexArray = new VertexArrayModel(_indices, _vertices, BufferUsageHint.StaticDraw);
            _vertexArray.Use();

            _shader = ShaderModel.FromUri(
                new Uri("/Shaders/Base.vert", UriKind.RelativeOrAbsolute),
                new Uri("/Shaders/Base.frag", UriKind.RelativeOrAbsolute),
                out bool success);

            if (!success)
            {
                // TODO: handle error
            }

            _editQueue = new GpuEditQueueModel(_vertexArray);
            _editQueue.Edits.Add(new GpuGrayscaleEditModel());
        }


        public void RenderToImage(GLWpfControl image)
        {
            var fb = _editQueue.Apply();

            if (fb is null)
            {
                return;
            }

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, image.Framebuffer);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // TODO: passing through the empty edit queue fucks up the image quality.
            // it also fucks up the panning sensetivity yay

            //fb.Data.Use(TextureUnit.Texture0);
            _texture?.Use(TextureUnit.Texture0);
            _shader.Use();
            _vertexArray.Use();
            GL.DrawElements(PrimitiveType.Triangles, _vertexArray.IndexCount, DrawElementsType.UnsignedInt, 0);
        }


        public void MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            float zoomDelta = (float)e.Delta > 0 ? .5f : -.5f;
            Point relative = e.GetPosition((IInputElement)sender);
            float relativeXScaled = 0, relativeYScaled = 0;
            if (sender is FrameworkElement frameworkElement)
            {
                relativeXScaled = (float)(relative.X / frameworkElement.ActualWidth) * 2 - 1;
                relativeYScaled = (float)(relative.Y / frameworkElement.ActualHeight) * 2 - 1;
            }

            float newZoom = zoom + (zoomDelta / zoomLevels);
            newZoom = Math.Clamp(newZoom, 0, 0.45f);
            if (newZoom == 0.5f) return;

            _panXStart = (_panXStart + relativeXScaled * zoom) - (relativeXScaled * newZoom);
            _panYStart = (_panYStart + relativeYScaled * zoom) - (relativeYScaled * newZoom);
            zoom = newZoom;

            Reload();
        }

        public void DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (sender is FrameworkElement frameworkElement)
            {
                var relativeXScaled = (float)(e.HorizontalChange / frameworkElement.ActualWidth);
                var relativeYScaled = (float)(e.VerticalChange / frameworkElement.ActualHeight);

                _panXDelta = relativeXScaled * (1 - zoom * 2) * panSensitivity;
                _panYDelta = relativeYScaled * (1 - zoom * 2) * panSensitivity;
            }

            Reload();
        }

        public void DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _panXStart += _panXDelta;
            _panYStart += _panYDelta;
            _panXDelta = _panYDelta = 0;

            Reload();
        }

        public void DragStarted(object sender, DragStartedEventArgs e)
        {
        }

        public void ResetZoom()
        {
            zoom = 0;
            _panXDelta = 0;
            _panYDelta = 0;
            _panXStart = 0;
            _panYStart = 0;

            Reload();
        }

        public void Reload()
        {
            float panX = _panXDelta + _panXStart;
            float panY = _panYDelta + _panYStart;

            float left      = 0.0f + zoom - panX;
            float right     = 1.0f - zoom - panX;
            float top       = 1.0f - zoom + panY;
            float bottom    = 0.0f + zoom + panY;

            float
                wNorm = (float)(BoundsSize.Width / RenderSize.Width),
                hNorm = (float)(BoundsSize.Height / RenderSize.Height);

            _vertices =
            [
                // Position             Texture coordinates
                 1.0f,  1.0f, 0.0f,     wNorm * right, hNorm * top,    // top right
                 1.0f, -1.0f, 0.0f,     wNorm * right, hNorm * bottom, // bottom right
                -1.0f, -1.0f, 0.0f,     wNorm * left,  hNorm * bottom, // bottom left
                -1.0f,  1.0f, 0.0f,     wNorm * left,  hNorm * top,    // top left
            ];

            _vertexArray.UpdateVertices(_vertices);
        }

        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _editQueue.Dispose();
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
