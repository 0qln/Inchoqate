using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.ViewModel;

public abstract class EditBaseLinearShader : EditBaseLinear, IDisposable
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<EditBaseLinearShader>();  

    protected static readonly ReadOnlyMemory<float> _vertices = (float[])
    [
        // Position             Texture coordinates
        1.0f,  1.0f, 0.0f,     1.0f, 1.0f, // top right
        1.0f, -1.0f, 0.0f,     1.0f, 0.0f, // bottom right
        -1.0f, -1.0f, 0.0f,     0.0f, 0.0f, // bottom left
        -1.0f,  1.0f, 0.0f,     0.0f, 1.0f  // top left
    ];

    protected static readonly ReadOnlyMemory<uint> _indices = (uint[])
    [
        0, 1, 3,
        1, 2, 3
    ];

    protected readonly ShaderModel? _shader;
    protected readonly VertexArrayModel _vao;

        
    public EditBaseLinearShader(BufferUsageHint usage = BufferUsageHint.StaticDraw)
    {
        _vao = new VertexArrayModel(_indices, _vertices, usage);
        _vao.Use();

        _shader = GetShader(out bool success);
        if (!success)
        {
            _logger.LogWarning("ShaderEditModel deriviant failed to generate the shader.");
        }
    }


    /// <summary>
    /// Generate the shader for this shader edit model.
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    public abstract ShaderModel? GetShader(out bool success);


    public override bool Apply(IEditDestinationModel destination, params IEditSourceModel[] sources)
    {
        if (destination is FrameBufferModel fb &&
            sources[0] is TextureModel tex)
        {
            return Apply(fb, tex);
        }

        return false;
    }

    public bool Apply(FrameBufferModel destination, TextureModel source)
    {
        if (_shader is null)
        {
            return false;
        }

        destination.UseAndClear(FramebufferTarget.Framebuffer);
        source.Use(TextureUnit.Texture0);
        _shader.Use();
        _vao.Use();
        GL.DrawElements(PrimitiveType.Triangles, _vao.IndexCount, DrawElementsType.UnsignedInt, 0);
        return true;
    }


    #region Clean up

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            _shader?.Dispose();
            _vao.Dispose();

            disposedValue = true;
        }
    }

    ~EditBaseLinearShader()
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
        GC.SuppressFinalize(this);
        Dispose(disposing: true);
    }

    #endregion
}