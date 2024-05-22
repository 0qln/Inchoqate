using Microsoft.Extensions.Logging;
using Inchoqate.Logging;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class VertexArrayModel : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<VertexArrayModel>();

        public readonly int Handle;


        public VertexArrayModel()
        {
            Handle = GL.GenVertexArray();
        }


        public void Use()
        {
            GL.BindVertexArray(Handle);
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteVertexArray(Handle);

                disposedValue = true;
            }
        }

        ~VertexArrayModel()
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
