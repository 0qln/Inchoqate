using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Windows;

namespace Inchoqate.GUI.Model
{
    /// <summary>
    /// </summary>
    /// <param name="vertexArray">This will not be disposed by this object.</param>
    public class HardwareEditQueueModel(VertexArrayModel vertexArray) : IDisposable, IRenderQueue
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<HardwareEditQueueModel>();

        // dispose
        private FrameBufferModel? _framebuffer1, _framebuffer2;
        public readonly List<IHardwareEdit> Edits = [];

        // no dispose
        private TextureModel? _sourceTexture;
        private readonly VertexArrayModel _vertexArrayObject = vertexArray;
        private Size _renderSize;


        public TextureModel? SourceTexture
        {
            get => _sourceTexture;
            set
            {
                _sourceTexture = value;
            }
        }


        /// <summary>
        /// The size in which the final output is rendered.
        /// </summary>
        public Size RenderSize
        {
            get => _renderSize;
            set
            {
                if (value == _renderSize) return;

                _renderSize = value;

                _framebuffer1?.Dispose();
                _framebuffer1 = new FrameBufferModel((int)value.Width, (int)value.Height, out bool success1);
                if (!success1)
                {
                    // TODO: handle error
                }

                _framebuffer1?.Dispose();
                _framebuffer2 = new FrameBufferModel((int)value.Width, (int)value.Height, out bool success2);
                if (!success2)
                {
                    // TODO: handle error
                }
            }
        }


        public FrameBufferModel? Apply()
        {
            if (_sourceTexture is null)
            {
                return null;
            }

            // If there are no edits given, return identity.
            if (Edits.Count == 0)
            {
                IHardwareEdit identity = new NoopHardwareEditModel();
                identity.Apply(
                    source: _sourceTexture,
                    destination: _framebuffer1!,
                    _vertexArrayObject);
                return _framebuffer1;
            }

            // Initial pass: fill framebuffer 1 with source texture.
            Edits.First().Apply(
                source: _sourceTexture,
                destination: _framebuffer1!,
                _vertexArrayObject);

            // Subsequent passes: switch between framebuffers.
            FrameBufferModel source = _framebuffer1!, destination = _framebuffer2!;
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

                foreach (var edit in Edits)
                {
                    edit?.Dispose();
                }

                disposedValue = true;
            }
        }

        ~HardwareEditQueueModel()
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
