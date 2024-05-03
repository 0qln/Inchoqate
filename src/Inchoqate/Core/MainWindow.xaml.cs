using OpenTK.Mathematics;
using OpenTK.Wpf;
using OpenTK.Graphics.OpenGL4;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System.IO;
using OpenTK.Compute.OpenCL;

namespace Core;


public partial class MainWindow : Window
{
    public static readonly ILoggerFactory LoggerFactory;

    static MainWindow()
    {
        string logFilePath = "log.txt";
        StreamWriter logFileWriter = new(logFilePath, append: true);
        LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddProvider(new FileLoggerProvider(logFileWriter));
        });
    }

    private readonly ILogger<MainWindow> _logger = LoggerFactory.CreateLogger<MainWindow>();
    private float[] Vertices;
    private uint[] Indices;
    private int ElementBufferObject;
    private int VertexBufferObject;
    private int VertexArrayObject;
    private Shader Shader;
    private Texture Texture;



    public MainWindow()
    {
        InitializeComponent();

        var settings = new GLWpfControlSettings
        {
            RenderContinuously = false,
        };
        OpenTkControl.Start(settings);


        Vertices = [
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
        ];
        
        Indices = [
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        ];

        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

        VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        VertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

        ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

        Shader = new Shader("ShaderBase.vert", "ShaderBase.frag");
        Shader.Use();

        int aPositionLoc = GL.GetAttribLocation(Shader.Handle, "aPosition");
        GL.EnableVertexAttribArray(aPositionLoc);
        GL.VertexAttribPointer(aPositionLoc, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        int aTexCoordLoc = GL.GetAttribLocation(Shader.Handle, "aTexCoord");
        GL.EnableVertexAttribArray(aTexCoordLoc);
        GL.VertexAttribPointer(aTexCoordLoc, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        Texture = new Texture("container.png");
        Texture.Use(TextureUnit.Texture0);
    }


    private void OpenTkControl_OnRender(TimeSpan delta)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);

        GL.BindVertexArray(VertexArrayObject);
        Texture.Use(TextureUnit.Texture0);

        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

        FramesDelta.Text = delta.Milliseconds.ToString();
    }
}