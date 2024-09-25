using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.Graphics;

public class FrameBuffer : IDisposable, IEditDestination
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<FrameBuffer>();

    public readonly int Handle;
    public readonly TextureModel Data;


    public FrameBuffer(TextureModel texture, out bool success)
    {    
        Handle = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

        Data = texture;
        Data.Use();

        GL.FramebufferTexture2D(
            FramebufferTarget.Framebuffer,
            FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D,
            Data.Handle,
            0);

        var successFrameBuffer = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        var errors = Logger.CheckErrors();

        success = !errors && successFrameBuffer == FramebufferErrorCode.FramebufferComplete;

        if (!success)
            Logger.LogError("Failed to generate frame buffer: Status:{s}", successFrameBuffer);
    }

    public FrameBuffer(PixelBuffer buffer, out bool success)
        : this(TextureModel.FromData(buffer.Width, buffer.Height, buffer.Data), out success)
    { 
    }

    public FrameBuffer(int width, int height, out bool success)
        : this(TextureModel.FromData(width, height), out success)
    {
    }


    public void Use(FramebufferTarget target)
    {
        GL.BindFramebuffer(target, Handle);

        Logger.CheckErrors("Failed to use frame buffer");
    }

    public void UseAndClear(FramebufferTarget target, ClearBufferMask? clear = ClearBufferMask.ColorBufferBit)
    {
        if (target == FramebufferTarget.ReadFramebuffer)
            throw new ArgumentException("Cannot clear a readonly buffer.");

        Use(target);
        if (clear is not null)
        {
            GL.ClearColor(0, 1, 0, 0);
            GL.Clear((ClearBufferMask)clear);

            Logger.CheckErrors("Failed to clear frame buffer");
        }
    }


    #region Clean up

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            Data.Dispose();
            GL.DeleteFramebuffer(Handle);
            _disposedValue = Logger.CheckErrors("Failed to delete frame buffer");
        }
    }

    ~FrameBuffer()
    {
        if (_disposedValue == false)
        {
            Logger.LogWarning("GPU Resource leak!");
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}