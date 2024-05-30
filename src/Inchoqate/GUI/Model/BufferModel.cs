using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Inchoqate.GUI.Model
{
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
            using (var pin = values.Pin())
            {
                // TODO: no idea if this will work
                IntPtr data = (IntPtr)pin.Pointer;
                IntPtr size = Size;
                GL.BufferData(bufferTarget, size, data, usage);
            }
        }

        public BufferModel(BufferTarget bufferTarget, Span<T> values, BufferUsageHint usage)
        {
            Target = bufferTarget;
            Size = values.Length * Marshal.SizeOf<T>();

            Handle = GL.GenBuffer();
            GL.BindBuffer(bufferTarget, Handle);
            GL.BufferData<T>(bufferTarget, Size, ref values[0], usage);
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

        ~BufferModel()
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
