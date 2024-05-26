using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows.Media;

namespace Inchoqate.GUI.Model
{
    public class FrameBufferModel : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<FrameBufferModel>();

        public readonly int Handle;
        public readonly TextureModel Data;


        public FrameBufferModel(int width, int height, out bool success, Color borderColor = default)
        {
            Handle = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Handle);

            Data = new TextureModel(width, height, borderColor);

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


        public void Use(FramebufferTarget target, ClearBufferMask? clear = ClearBufferMask.ColorBufferBit)
        {
            GL.BindFramebuffer(target, Handle);
            if (clear is not null)
            {
                GL.ClearColor(0, 1, 0, 0);
                GL.Clear((ClearBufferMask)clear);
            }
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

        ~FrameBufferModel()
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
