using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;

namespace Inchoqate.GUI.ViewModel
{
    public class StackEditorViewModel : RenderEditorViewModel, IDisposable 
    {
        private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<StackEditorViewModel>();


        private FrameBufferModel? _framebuffer1, _framebuffer2;
        private TextureModel? _sourceTexture;
        private EditorNodeCollectionLinear? _edits;


        public EditorNodeCollectionLinear? Edits
        {
            get => _edits;
            set
            {
                if (_edits is not null) _edits.CollectionChanged -= Edits_CollectionChanged;
                if (_edits is not null) _edits.ItemsPropertyChanged -= Edits_ItemsPropertyChanged;
                SetProperty(ref _edits, value);
                _edits!.CollectionChanged += Edits_CollectionChanged;
                _edits!.ItemsPropertyChanged += Edits_ItemsPropertyChanged;
            }
        }

        void Edits_ItemsPropertyChanged(object? sender, EventArgs e)
        {
            Invalidate();
        }

        void Edits_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Invalidate();
        }


        public override EventRelayViewModel EditsProvider => new(_edits, EventTree);

        public override EventTreeViewModel EventTree { get; } = new("Stack Editor");


        public StackEditorViewModel()
        {
            Edits = _edits = new(relayTarget: EventTree);

            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(RenderSize):
                        ReloadBuffer(ref _framebuffer1);
                        ReloadBuffer(ref _framebuffer2);
                        break;
                    case nameof(VoidColor):
                        if (_framebuffer1 is not null) _framebuffer1.Data.BorderColor = VoidColor;
                        if (_framebuffer2 is not null) _framebuffer2.Data.BorderColor = VoidColor;
                        break;
                }
            };
        }


        // TODO: if the new size is smaller, don't dispose and just use a subset of the buffer.
        private void ReloadBuffer(ref FrameBufferModel? buffer)
        {
            buffer?.Dispose();
            buffer = new FrameBufferModel((int)_renderSize.Width, (int)_renderSize.Height, out bool success1);
            if (!success1)
            {
                _logger.LogError("Failed to create framebuffer.");
                return;
            }
            buffer.Data.BorderColor = VoidColor;
        }


        public override bool Compute()
        {
            if (_sourceTexture is null || _edits is null)
                return false;

            // If there are no edits given, return identity.
            if (_edits.Count == 0)
            {
                EditImplIdentityViewModel identity = new();
                identity.Apply(_framebuffer1!, _sourceTexture);
                Result = _framebuffer1;
                return true;
            }

            FrameBufferModel source = _framebuffer1!, destination = _framebuffer2!;
            var edits = _edits.ToList();

            // Initial pass: load source texture.
            if (!edits.First().Apply(destination, _sourceTexture))
            {
                return false;
            }

            // Subsequent passes: switch between framebuffers.
            foreach (var edit in edits[1..])
            {
                (source, destination) = (destination, source);
                if (!edit.Apply(destination, source.Data))
                {
                    return false;
                }
            }

            Result = destination;
            return true;
        }


        public override void Invalidate()
        {
            Result = null;
        }


        public override void SetSource(TextureModel? value)
        {
            if (value == _sourceTexture)
            {
                return;
            }

            _sourceTexture?.Dispose();
            _sourceTexture = value;
            SourceSize = new(_sourceTexture?.Width ?? 0, _sourceTexture?.Height ?? 0);
            Invalidate();
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

                if (_edits is not null)
                {
                    foreach (var edit in _edits)
                    {
                        if (edit is IDisposable disposable)
                            disposable.Dispose();
                    }
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
