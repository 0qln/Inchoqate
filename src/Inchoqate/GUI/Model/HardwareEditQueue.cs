using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace GUI.Model
{
    public class HardwareEditQueue : IDisposable
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<HardwareEditQueue>();

        // dispose
        private readonly FrameBufferModel _framebuffer1, _framebuffer2;

        // no dispose
        private readonly TextureModel _sourceTexture;
        private readonly VertexArrayModel _vertexArrayObject;
        public readonly List<IHardwareEdit> Edits = [];


        /// <summary>
        /// Create a new change queue.
        /// </summary>
        /// <param name="source">This will not be disposed by this object.</param>
        /// <param name="vertexArray">This will not be disposed by this object.</param>
        public HardwareEditQueue(TextureModel source, VertexArrayModel vertexArray)
        {
            _framebuffer1 = new FrameBufferModel(source.Width, source.Height, out bool success1);
            if (!success1)
            {
                // TODO: handle error
            }

            _framebuffer2 = new FrameBufferModel(source.Width, source.Height, out bool success2);
            if (!success2)
            {
                // TODO: handle error
            }

            _sourceTexture = source;
            _vertexArrayObject = vertexArray;
        }


        /// <summary>
        /// Returns a reference to the final result of the render queue.
        /// </summary>
        /// <returns></returns>
        public FrameBufferModel Apply()
        {
            // Initial pass: fill framebuffer 1 with source texture.
            Edits.First().Apply(
                source: _sourceTexture,
                destination: _framebuffer1,
                _vertexArrayObject);

            // Subsequent passes: switch between framebuffers.
            FrameBufferModel source = _framebuffer1, destination = _framebuffer2;
            foreach (var edit in Edits[1..])
            {
                edit.Apply(source.Data, destination, _vertexArrayObject);
                (source, destination) = (destination, source);
            }

            // Return result.
            return destination;
        }


        #region Clean up

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                _framebuffer1?.Dispose();
                _framebuffer1?.Dispose();

                disposedValue = true;
            }
        }

        ~HardwareEditQueue()
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
