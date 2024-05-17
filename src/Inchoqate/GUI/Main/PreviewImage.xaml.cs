using Inchoqate.Miscellaneous;
using OpenTK.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK.Graphics.OpenGL4;
using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using System.Windows.Media.TextFormatting;
using GUI;
using System.Reflection.Metadata;

namespace Inchoqate.GUI.Main
{
    /// <summary>
    /// Interaction logic for PreviewImage.xaml
    /// </summary>
    public partial class PreviewImage : UserControl, IDisposable
    {
        private readonly ILogger<PreviewImage> _logger = FileLoggerFactory.CreateLogger<PreviewImage>();


        private readonly Buffer<uint> _elementBufferObject;

        private readonly Buffer<float> _vertexBufferObject;

        private readonly VertexArray _vertexArrayObject;

        private Shader? _shader1;

        public Shader? Shader1
        {
            get
            {
                return _shader1;
            }
            set
            {
                if(value is null)
                {
                    return;
                }

                // Dispose old shader
                _shader1?.Dispose();

                // Set up shader
                _shader1 = value;
                _shader1.Use();
                SetupVertexAttribs(_shader1);

                // Force a new rendering
                OpenTkControl.InvalidateVisual();
                UpdateTextureRenderSize(ActualWidth, ActualHeight);
            }
        }

        private Shader? _shader2;

        public Shader? Shader2
        {
            get
            {
                return _shader1;
            }
            set
            {
                if (value is null)
                {
                    return;
                }

                // Dispose old shader
                _shader2?.Dispose();

                // Set up shader
                _shader2 = value;
                _shader2.Use();
                SetupVertexAttribs(_shader2);

                // Force a new rendering
                OpenTkControl.InvalidateVisual();
                UpdateTextureRenderSize(ActualWidth, ActualHeight);
            }
        }


        // TODO: check if shader has the required attributes.
        private void SetupVertexAttribs(Shader shader)
        {
            int aPositionLoc = GL.GetAttribLocation(shader.Handle, "aPosition");
            GL.EnableVertexAttribArray(aPositionLoc);
            GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int aTexCoordLoc = GL.GetAttribLocation(shader.Handle, "aTexCoord");
            GL.EnableVertexAttribArray(aTexCoordLoc);
            GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }


        private Texture? _texture;

        public Texture? Texture
        {
            get
            {
                return _texture;
            }
            set
            {
                if (value is null)
                {
                    return;
                }

                // TODO: do we need to recreate the framebuffer here?

                // Dispose old data.
                _texture?.Dispose();
                _framebuffer?.Dispose();

                // Set up texture.
                _texture = value;
                _texture.Use(TextureUnit.Texture0);
                _framebuffer = new FrameBuffer((int)E_Border.ActualWidth, (int)E_Border.ActualHeight, out var success);
                if (!success)
                {
                    _framebuffer.Dispose();
                    // TODO: handle error
                }

                // Force a new rendering.
                OpenTkControl.InvalidateVisual();
                UpdateTextureRenderSize(ActualWidth, ActualHeight);
            }
        }


        private FrameBuffer? _framebuffer;


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


        public PreviewImage()
        {
            InitializeComponent();

            OpenTkControl.Start(new GLWpfControlSettings
            {
                RenderContinuously = false,
            });

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexArrayObject = new VertexArray();
            _vertexArrayObject.Use();

            _vertexBufferObject = new Buffer<float>(BufferTarget.ArrayBuffer, _vertices, BufferUsageHint.StaticDraw);
            _vertexBufferObject.Use();

            _elementBufferObject = new Buffer<uint>(BufferTarget.ElementArrayBuffer, _indices, BufferUsageHint.StaticDraw);
            _elementBufferObject.Use();

            Shader1 = new Shader(BuildFiles.Get("Shaders/ShaderBase.vert"), BuildFiles.Get("Shaders/NoRed.frag"), out var success1);
            if (!success1)
            {
                Shader1.Dispose();
                // TODO: handle error
            }

            Shader2 = new Shader(BuildFiles.Get("Shaders/Identity.vert"), BuildFiles.Get("Shaders/NoGreen.frag"), out var success2);
            if (!success2)
            {
                Shader2.Dispose();
                // TODO: handle error
            }
        }

        private void OpenTkControl_OnRender(TimeSpan obj)
        {
            if (_texture is null || _framebuffer is null || _shader1 is null || _shader2 is null)
            {
                return;
            }

            // Pass 1
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer.Handle);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _texture.Use(TextureUnit.Texture0);
            _shader1.Use();
            _vertexArrayObject.Use();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            // Pass 2
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, OpenTkControl.Framebuffer);
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _framebuffer.Data.Use(TextureUnit.Texture0);
            _shader2.Use();
            _vertexArrayObject.Use();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTextureRenderSize(e.NewSize.Width, e.NewSize.Height);
        }

        private void UpdateTextureRenderSize(double boundsX, double boundsY)
        {
            if (_texture is null)
            {
                return;
            }

            double aspectRatio = (double)_texture.Height / _texture.Width;
            double boundsRatio = boundsY / boundsX;

            OpenTkControl.Width = boundsX;
            OpenTkControl.Height = boundsY;

            if (boundsRatio > aspectRatio)
            {
                OpenTkControl.Height = boundsX * aspectRatio;
            }
            else if (boundsRatio < aspectRatio)
            {
                OpenTkControl.Width = boundsY / aspectRatio;
            }
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _elementBufferObject.Dispose();
                _vertexBufferObject.Dispose();
                _vertexArrayObject.Dispose();
                _texture?.Dispose();
                _framebuffer?.Dispose();
                _shader1?.Dispose();
                _shader2?.Dispose();

                disposedValue = true;
            }
        }

        ~PreviewImage()
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
