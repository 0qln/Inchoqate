using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI
{
    public class FrameBuffer : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<FrameBuffer>();

        public readonly int Handle;
        public readonly Texture Data;


        public FrameBuffer(int width, int height, out bool success)
        {
            Handle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

            Data = new Texture(width, height);

            GL.FramebufferTexture2D(
                FramebufferTarget.Framebuffer, 
                FramebufferAttachment.ColorAttachment0, 
                TextureTarget.Texture2D, 
                Data.Handle, 
                0);

            var successFramebuffer = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
            if (successFramebuffer != FramebufferErrorCode.FramebufferComplete)
            {
                _logger.LogError(
                    "OpenGL error while generating framebuffer: Code:{error} | Status:{successFramebuffer}",
                    GL.GetError(),
                    successFramebuffer);
                success = false;
                goto clean_up;
            }

            success = true;

            clean_up:
            return;
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Data.Dispose();
                GL.DeleteFramebuffer(Handle);

                disposedValue = true;
            }
        }

        ~FrameBuffer()
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
