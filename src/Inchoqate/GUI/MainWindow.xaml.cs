using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using OpenTK.Wpf;
using System.Diagnostics;
using System.Text;
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
using System.IO;
using OpenTK.Compute.OpenCL;
using Inchoqate.Miscellaneous;
using Utillities.Wpf;

namespace Inchoqate.GUI
{
    public partial class MainWindow : Window
    {
        private readonly ILogger<MainWindow> _logger = FileLoggerFactory.CreateLogger<MainWindow>();
        private readonly WindowHandle _windowHandle;


        private readonly float[] _vertices =
        {
            // Position         Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        };

        private readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int _elementBufferObject;

        private int _vertexBufferObject;

        private int _vertexArrayObject;

        private Shader _shader;

        private Texture _texture;


        public MainWindow()
        {
            BuildFiles.Initiate(clearOldData: true);


            InitializeComponent();


            _windowHandle = new WindowHandle(this);


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

            _shader = new Shader(BuildFiles.Get("Shaders/ShaderBase.vert"), BuildFiles.Get("Shaders/ShaderBase.frag"));
            _shader.Use();

            int aPositionLoc = GL.GetAttribLocation(_shader.Handle, "aPosition");
            GL.EnableVertexAttribArray(aPositionLoc);
            GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int aTexCoordLoc = GL.GetAttribLocation(_shader.Handle, "aTexCoord");
            GL.EnableVertexAttribArray(aTexCoordLoc);
            GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _texture = new Texture(BuildFiles.Get("Sample-Images/img0/1920x1080.bmp"));
            _texture.Use(TextureUnit.Texture0);
        }


        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BindVertexArray(_vertexArrayObject);

            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}