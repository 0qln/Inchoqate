using Inchoqate.GUI.Logging;
using Inchoqate.GUI.Model;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.ViewModel;

public abstract class EditBaseLinearShader : EditBaseLinear, IEdit<Texture, FrameBuffer>, IDisposable
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<EditBaseLinearShader>();  

    protected static readonly ReadOnlyMemory<float> Vertices = (float[])
    [
        // Position             Texture coordinates
        1.0f,  1.0f, 0.0f,     1.0f, 1.0f, // top right
        1.0f, -1.0f, 0.0f,     1.0f, 0.0f, // bottom right
        -1.0f, -1.0f, 0.0f,     0.0f, 0.0f, // bottom left
        -1.0f,  1.0f, 0.0f,     0.0f, 1.0f  // top left
    ];

    protected static readonly ReadOnlyMemory<uint> Indices = (uint[])
    [
        0, 1, 3,
        1, 2, 3
    ];

    protected readonly Shader? Shader;
    protected readonly VertexArray Vao;


    protected EditBaseLinearShader(BufferUsageHint usage = BufferUsageHint.StaticDraw)
    {
        Vao = new(Indices, Vertices, usage);
        Vao.Use();

        // TODO: fix virtual member call in constructor
        Shader = GetShader(out var success);
        if (!success)
        {
            Logger.LogError("Deriving class failed to generate the shader.");
        }
    }


    /// <summary>
    /// Generate the shader for this shader edit model.
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    public abstract Shader? GetShader(out bool success);


    public override bool Apply()
    {
        if (Shader is null || Sources is null || Destination is null)
        {
            Logger.LogWarning("Missing shader or sources or destination.");
            return false;
        }

        Destination.UseAndClear(FramebufferTarget.Framebuffer);
        Sources[0].Use(TextureUnit.Texture0);
        Shader.Use();
        Vao.Use();
        GL.DrawElements(PrimitiveType.Triangles, Vao.IndexCount, DrawElementsType.UnsignedInt, 0);
        return true;
    }

    /// <inheritdoc />
    public FrameBuffer? Destination { get; set; }

    /// <inheritdoc />
    public Texture[]? Sources { get; set; }


    #region Clean up

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            Shader?.Dispose();
            Vao.Dispose();

            _disposed = true;
        }
    }

    ~EditBaseLinearShader()
    {
        // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
        // The OpenGL resources have to be released from a thread with an active OpenGL Context.
        // The GC runs on a separate thread, thus releasing unmanaged GL resources inside the finalizer
        // is not possible.
        if (_disposed == false)
        {
            Logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(disposing: true);
    }

    #endregion
}