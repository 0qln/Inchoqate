using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using MvvmHelpers;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI.ViewModel
{
    public class StackEditorViewModel : BaseViewModel, IDisposable, IEditorModel<TextureModel, FrameBufferModel>
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<StackEditorViewModel>();

        private readonly EditorNodeCollectionLinear _edits = new();
        public EditorNodeCollectionLinear Nodes
        {
            get => _edits;
        }
        public event NotifyCollectionChangedEventHandler? EditsChanged;

        private FrameBufferModel? _framebuffer1, _framebuffer2;
        private TextureModel? _sourceTexture;

        public void SetSource(TextureModel? value)
        {
            _sourceTexture?.Dispose();
            _sourceTexture = value;
        }

        private Color _voidColor;
        private Size _renderSize;

        public Color VoidColor
        {
            get
            {
                return _voidColor;
            }
            set
            {
                if (value == _voidColor) return;
                _voidColor = value;
                if (_framebuffer1 is not null)
                    _framebuffer1.Data.BorderColor = value;
                if (_framebuffer2 is not null)
                    _framebuffer2.Data.BorderColor = value;
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
                Reload();
            }
        }

        public void Reload()
        {
            // TODO: if the new size is smaller, don't dispose and just use a subset of the buffer.

            _framebuffer1?.Dispose();
            _framebuffer1 = new FrameBufferModel((int)_renderSize.Width, (int)_renderSize.Height, out bool success1);
            if (!success1)
                // TODO: handle error
                return;
            _framebuffer1.Data.BorderColor = VoidColor;

            _framebuffer2?.Dispose();
            _framebuffer2 = new FrameBufferModel((int)_renderSize.Width, (int)_renderSize.Height, out bool success2);
            if (!success2)
                // TODO: handle error
                return;
            _framebuffer2.Data.BorderColor = VoidColor;
        }


        public StackEditorViewModel()
        {
            _edits.CollectionChanged += (s, e) => EditsChanged?.Invoke(s, e);
        }


        public FrameBufferModel? Compute(out bool success)
        {
            if (_sourceTexture is null)
            {
                success = false;
                return null;
            }

            // If there are no edits given, return identity.
            if (_edits.Count == 0)
            {
                EditImplIdentityViewModel identity = new();
                identity.Apply(_framebuffer1!, _sourceTexture);
                success = true;
                return _framebuffer1;
            }

            FrameBufferModel source = _framebuffer1!, destination = _framebuffer2!;
            var edits = _edits.ToList();

            // Initial pass: load source texture.
            if (!edits.First().Apply(destination, _sourceTexture))
            {
                success = false;
                return null;
            }

            // Subsequent passes: switch between framebuffers.
            foreach (var edit in edits[1..])
            {
                (source, destination) = (destination, source);
                if (!edit.Apply(destination, source.Data))
                {
                    success = false;
                    return null;
                }
            }

            // Return result.
            success = true;
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

                foreach (var edit in _edits)
                {
                    if (edit is IDisposable disposable)
                        disposable.Dispose();
                }

                disposedValue = true;
            }
        }

        ~StackEditorViewModel()
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
