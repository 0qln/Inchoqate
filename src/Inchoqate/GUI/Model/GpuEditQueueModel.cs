using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Windows;
using OpenTK.Graphics.OpenGL4;

namespace Inchoqate.GUI.Model
{
    public class GpuEditQueueModel : IDisposable, IRenderQueue
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<GpuEditQueueModel>();

        public readonly List<IGPUEdit> Edits = [];
        private FrameBufferModel? _framebuffer1, _framebuffer2;
        private TextureModel? _sourceTexture;

        public TextureModel? SourceTexture
        {
            get => _sourceTexture;
            set
            {
                _sourceTexture?.Dispose();
                _sourceTexture = value;
            }
        }

        private Size _renderSize;

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

                _framebuffer2?.Dispose();
                _framebuffer2 = new FrameBufferModel((int)value.Width, (int)value.Height, out bool success2);
                if (!success2)
                {
                    // TODO: handle error
                }
            }
        }


        public FrameBufferModel? Apply()
        {
            if (_sourceTexture is null || _framebuffer1 is null || _framebuffer2 is null)
            {
                return null;
            }

            // If there are no edits given, return identity.
            if (Edits.Count == 0)
            {
                IGPUEdit identity = new GpuIdentityEditModel();
                identity.Apply(_sourceTexture, _framebuffer1!);
                return _framebuffer1;
            }

            FrameBufferModel source = _framebuffer1, destination = _framebuffer2;

            // Initial pass: load source texture.
            Edits.First().Apply(_sourceTexture, destination);

            // Subsequent passes: switch between framebuffers.
            foreach (var edit in Edits[1..])
            {
                (source, destination) = (destination, source);
                edit.Apply(source.Data, destination);
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
                _sourceTexture?.Dispose();

                foreach (var edit in Edits)
                {
                    edit?.Dispose();
                }

                disposedValue = true;
            }
        }

        ~GpuEditQueueModel()
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
