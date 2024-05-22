using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Inchoqate.GUI
{
    public class Buffer<T> : IDisposable
        where T : struct
    {
        private readonly ILogger _logger = FileLoggerFactory.CreateLogger<Buffer<T>>();

        public readonly int Handle;

        public readonly BufferTarget Target;


        public Buffer(BufferTarget bufferTarget, T[] values, BufferUsageHint usage)
        {
            Target = bufferTarget;

            Handle = GL.GenBuffer();
            GL.BindBuffer(bufferTarget, Handle);
            GL.BufferData(bufferTarget, values.Length * Marshal.SizeOf<T>(), values, usage);
        }


        public void Use()
        {
            GL.BindBuffer(Target, Handle);
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteBuffer(Handle);

                disposedValue = true;
            }
        }

        ~Buffer()
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
