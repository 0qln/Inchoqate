using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Inchoqate.GUI.Model;

public class BufferModel<T> : IDisposable
    where T : struct
{
    private readonly ILogger _logger = FileLoggerFactory.CreateLogger<BufferModel<T>>();

    public readonly int Handle;

    public readonly BufferTarget Target;
    public readonly int Size;


    public unsafe BufferModel(BufferTarget bufferTarget, ReadOnlyMemory<T> values, BufferUsageHint usage)
    {
        Target = bufferTarget;
        Size = values.Length * Marshal.SizeOf<T>();

        Handle = GL.GenBuffer();
        GL.BindBuffer(bufferTarget, Handle);
        using var pin = values.Pin();
        GL.BufferData(bufferTarget, (IntPtr)Size, (IntPtr)pin.Pointer, usage);

        if (GraphicsModel.CheckErrors())
            _logger.LogError("Failed to generate buffer.");
    }

    public BufferModel(BufferTarget bufferTarget, Span<T> values, BufferUsageHint usage)
    {
        Target = bufferTarget;
        Size = values.Length * Marshal.SizeOf<T>();

        Handle = GL.GenBuffer();
        GL.BindBuffer(bufferTarget, Handle);
        GL.BufferData(bufferTarget, Size, ref values[0], usage);

        if (GraphicsModel.CheckErrors())
            _logger.LogError("Failed to generate buffer.");
    }


    /// <summary>
    /// Update the buffers data.
    /// </summary>
    /// <param name="data">The new data.</param>
    /// <param name="offset">Offset of the data measured in bytes.</param>
    /// <exception cref="ArgumentException"></exception>
    public void Update(T[] data, int offset = 0)
    {
        if (data.Length * Marshal.SizeOf<T>() + offset > Size)
        {
            throw new ArgumentException(nameof(data.Length));
        }

        Use();
        GL.BufferSubData(Target, offset, Size, data);

        if (GraphicsModel.CheckErrors())
            _logger.LogError("Failed to update buffer.");
    }


    public void Use()
    {
        GL.BindBuffer(Target, Handle);

        if (GraphicsModel.CheckErrors())
            _logger.LogError("Failed to update buffer.");
    }


    #region Clean up

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            GL.DeleteBuffer(Handle);
            _disposed = !GraphicsModel.CheckErrors("Failed to delete buffer.", _logger);
        }
    }

    ~BufferModel()
    {
        if (_disposed == false)
        {
            _logger.LogWarning("GPU Resource leak!");
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}