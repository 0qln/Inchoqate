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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK.Graphics.OpenGL4;
using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using System.Windows.Media.TextFormatting;

namespace Inchoqate.GUI.Main
{
    /// <summary>
    /// Interaction logic for PreviewImage.xaml
    /// </summary>
    public partial class PreviewImage : UserControl
    {
        private readonly ILogger<PreviewImage> _logger = FileLoggerFactory.CreateLogger<PreviewImage>();


        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader? _shader;

        public Shader? Shader
        {
            get
            {
                return _shader;
            }
            set
            {
                if(value is null)
                {
                    return;
                }

                _shader = value;
                _shader.Use();

                int aPositionLoc = GL.GetAttribLocation(_shader.Handle, "aPosition");
                GL.EnableVertexAttribArray(aPositionLoc);
                GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                int aTexCoordLoc = GL.GetAttribLocation(_shader.Handle, "aTexCoord");
                GL.EnableVertexAttribArray(aTexCoordLoc);
                GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

                // Force a new rendering
                OpenTkControl.InvalidateVisual();
                UpdateTextureRenderSize(ActualWidth, ActualHeight);
            }
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

                _texture = value;
                _texture.Use(TextureUnit.Texture0);

                // Force a new rendering
                OpenTkControl.InvalidateVisual();
                UpdateTextureRenderSize(ActualWidth, ActualHeight);
            }
        }


        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             1.0f,  1.0f, 0.0f, 1.0f, 1.0f, // top right
             1.0f, -1.0f, 0.0f, 1.0f, 0.0f, // bottom right
            -1.0f, -1.0f, 0.0f, 0.0f, 0.0f, // bottom left
            -1.0f,  1.0f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };


        public PreviewImage()
        {
            InitializeComponent();

            var settings = new GLWpfControlSettings
            {
                RenderContinuously = false,
            };
            OpenTkControl.Start(settings);

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            Shader = new Shader(BuildFiles.Get("Shaders/ShaderBase.vert"), BuildFiles.Get("Shaders/Grayscale.frag"));
        }

        private void OpenTkControl_OnRender(TimeSpan obj)
        {
            if (_texture is null || _shader is null)
            {
                return;
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

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
    }
}
