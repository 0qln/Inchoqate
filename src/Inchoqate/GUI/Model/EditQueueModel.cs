using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using OpenTK.Graphics.OpenGL4;
using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI.Model
{
    public class EditQueueModel<TSource> : IGpuRenderQueue, ICpuRenderQueue, IDisposable
        where TSource : IDisposable
    {
        private readonly ILogger _logger = FileLoggerFactory.CreateLogger<EditQueueModel<TSource>>();

        private bool disposedValue;

        public readonly List<IEdit> Edits = [];
        private FrameBufferModel? _framebuffer1, _framebuffer2;
        private PixelBufferModel? _pixelbuffer1, _pixelbuffer2;


        private TSource? _source;

        public TSource? SourceData
        {
            get => _source;
            set
            {
                _source?.Dispose();
                _source = value;
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
                // TODO: may not need to reload here, just set the border color.
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



        FrameBufferModel? IGpuRenderQueue.Apply()
        {
            throw new NotImplementedException();
        }

        PixelBufferModel? ICpuRenderQueue.Apply()
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _pixelbuffer1?.Dispose();
                    _pixelbuffer2?.Dispose();
                    _framebuffer1?.Dispose();
                    _framebuffer2?.Dispose();
                    _source?.Dispose();
                }

                disposedValue = true;
            }
        }

        ~EditQueueModel()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);

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
    }
}
