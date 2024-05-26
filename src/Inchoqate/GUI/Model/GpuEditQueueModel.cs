using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Windows;
using OpenTK.Graphics.OpenGL4;
using System.Windows.Media;

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

        private Color _background;

        public Color Background
        {
            get
            {
                return _background;
            }
            set
            {
                if (value == _background) return;
                _background = value;
                Reload();
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
                Reload();
            }
        }


        public void Reload()
        {
            // TODO: if the new size is smaller, don't dispose and just use a subset of the buffer.

            _framebuffer1?.Dispose();
            _framebuffer1 = new FrameBufferModel((int)_renderSize.Width, (int)_renderSize.Height, out bool success1, Background);
            if (!success1)
            {
                // TODO: handle error
            }

            _framebuffer2?.Dispose();
            _framebuffer2 = new FrameBufferModel((int)_renderSize.Width, (int)_renderSize.Height, out bool success2, Background);
            if (!success2)
            {
                // TODO: handle error
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
