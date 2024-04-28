using OpenTK.Mathematics;
using OpenTK.Wpf;
using OpenTK.Graphics.OpenGL;
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

namespace Core
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var settings = new GLWpfControlSettings
            {
                RenderContinuously = false,
            };
            OpenTkControl.Start(settings);

            _texture = new Texture(@"D:\Programmmieren\Projects\Connaisseur\ImageConnaisseur\src\ImageConnaisseur\Misc\Images\sample-255x255.png");

            Vertices = [
                -0.5f, -0.5f, 0.0f, //Bottom-left vertex
                 0.5f, -0.5f, 0.0f, //Bottom-right vertex
                 0.0f,  0.5f, 0.0f  //Top vertex
            ];
            VertexBufferObject = GL.GenBuffer();

            Shader = new Shader("ShaderBase.vert", "ShaderBase.frag");

            VertexArrayObject = GL.GenVertexArray();

            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);


        }

        int VertexArrayObject;
        int VertexBufferObject;
        float[] Vertices;
        Shader Shader;

        private Texture _texture;

        private void OpenTkControl_OnRender(TimeSpan delta)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            Shader.Use();
            GL.BindVertexArray(VertexArrayObject);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            FPS.Text = delta.Milliseconds.ToString();
        }
    }
}