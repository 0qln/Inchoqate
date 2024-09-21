using Microsoft.Extensions.Logging;
using Inchoqate.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model;

public class VertexArrayModel : IDisposable
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<VertexArrayModel>();

    public readonly int Handle;
    public readonly int IndexCount;

    private readonly BufferModel<uint> _elementBufferObject;
    private readonly BufferModel<float> _vertexBufferObject;


    /// <summary>
    /// Create a new vertex array object (VAO).
    /// </summary>
    /// <param name="mIndx">The indices.</param>
    /// <param name="mVert">The verteces.</param>
    /// <param name="usage"></param>
    public VertexArrayModel(ReadOnlyMemory<uint> mIndx, ReadOnlyMemory<float> mVert, BufferUsageHint usage)
    {
        Handle = GL.GenVertexArray();

        if (GraphicsModel.CheckErrors())
            Logger.LogError("Failed to create vertex array.");

        Use();

        _vertexBufferObject = new BufferModel<float>(BufferTarget.ArrayBuffer, mVert, usage);
        _vertexBufferObject.Use();

        _elementBufferObject = new BufferModel<uint>(BufferTarget.ElementArrayBuffer, mIndx, usage);
        _elementBufferObject.Use();

        IndexCount = mIndx.Length;
    }

    /// <summary>
    /// Create a new vertex array object (VAO).
    /// </summary>
    /// <param name="sIndx">The indices.</param>
    /// <param name="sVert">The verteces.</param>
    /// <param name="usage"></param>
    public VertexArrayModel(Span<uint> sIndx, Span<float> sVert, BufferUsageHint usage)
    {
        Handle = GL.GenVertexArray();

        if (GraphicsModel.CheckErrors())
            Logger.LogError("Failed to create vertex array.");

        Use();

        _vertexBufferObject = new BufferModel<float>(BufferTarget.ArrayBuffer, sVert, usage);
        _vertexBufferObject.Use();

        _elementBufferObject = new BufferModel<uint>(BufferTarget.ElementArrayBuffer, sIndx, usage);
        _elementBufferObject.Use();

        IndexCount = sIndx.Length;
    }
        

    public void UpdateVertices(float[] vertices)
    {
        _vertexBufferObject.Update(vertices);
    }

    public void UpdateIndices(uint[] indices)
    {
        _elementBufferObject.Update(indices);
    }


    public void Use()
    {
        GL.BindVertexArray(Handle);

        if (GraphicsModel.CheckErrors())
        {
            Logger.LogError("Failed to use vertex array.");
        }
    }


    #region Clean up

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            GL.DeleteVertexArray(Handle);
            _elementBufferObject.Dispose();
            _vertexBufferObject.Dispose();
            _disposedValue = !GraphicsModel.CheckErrors("Failed to delete vertex array.", Logger);
        }
    }

    ~VertexArrayModel()
    {
        if (_disposedValue == false)
        {
            Logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}