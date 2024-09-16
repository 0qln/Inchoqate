using System.Windows.Controls;
using Inchoqate.GUI.Model;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel;

public class StackEditorViewModel : RenderEditorViewModel, IDisposable
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<StackEditorViewModel>();

    private FrameBufferModel? _framebuffer1, _framebuffer2;
    private PixelBufferModel? _pixelBuffer1, _pixelBuffer2;
    private TextureModel? _sourceTexture;
    private readonly EditorNodeCollectionLinear _edits;


    public EditorNodeCollectionLinear Edits => _edits;

    public sealed override EventTreeViewModel EventTree { get; }


    protected override void HandlePropertyChanged(string? propertyName)
    {
        base.HandlePropertyChanged(propertyName);

        switch (propertyName)
        {
            case nameof(RenderSize):
                ReloadBuffer(ref _framebuffer1);
                ReloadBuffer(ref _framebuffer2);
                ReloadBuffer(ref _pixelBuffer1);
                ReloadBuffer(ref _pixelBuffer2);
                break;
            case nameof(VoidColor):
                if (_framebuffer1 is not null) _framebuffer1.Data.BorderColor = VoidColor;
                if (_framebuffer2 is not null) _framebuffer2.Data.BorderColor = VoidColor;
                break;
        }
    }

    public StackEditorViewModel()
        : this(new("Stack Editor"))
    {
    }

    public StackEditorViewModel(EventTreeViewModel eventTree)
    {
        EventTree = eventTree;
        _edits = new(relayTarget: EventTree);

        // Inject dependencies.
        foreach (var @event in EventTree.EnumerateSubtree())
        {
            switch (@event)
            {
                case IDependencyInjected<ICollection<EditBaseLinear>> a:
                    a.Dependency = _edits;
                    break;
                case IDependencyInjected<MonitoredObservableItemCollection<EditBaseLinear>> a:
                    a.Dependency = _edits;
                    break;
                case IDependencyInjected<IMoveItemsWrapper> a:
                    a.Dependency = _edits;
                    break;
            }
        }

        // Apply events.
        foreach (var @event in EventTree.EnumerateExecutedEvents())
        {
            @event.Do();
        }

        _edits.CollectionChanged += (_, _) => Invalidate();
        _edits.ItemsPropertyChanged += (_, _) => Invalidate();
    }


    // TODO: if the new size is smaller, don't dispose and just use a subset of the buffer.
    private void ReloadBuffer(ref FrameBufferModel? buffer)
    {
        buffer?.Dispose();
        buffer = new((int)RenderSize.Width, (int)RenderSize.Height, out var success);
        if (!success)
        {
            _logger.LogError("Failed to create framebuffer.");
            return;
        }

        buffer.Data.BorderColor = VoidColor;
    }

    private void ReloadBuffer(ref PixelBufferModel? buffer)
    {
        buffer?.Dispose();
        buffer = new(
            (int)RenderSize.Width * (int)RenderSize.Height * TextureModel.PixelDepth,
            (int)RenderSize.Width, (int)RenderSize.Height);
    }


    private readonly Lazy<EditImplIdentityViewModel> _identity = new(() => new());


    public override bool Compute()
    {
        if (_sourceTexture is null)
        {
            return false;
        }

        // If there are no edits given, return identity.
        if (_edits.Count == 0)
        {
            _identity.Value.Sources = [_sourceTexture];
            _identity.Value.Destination = _framebuffer1!;
            _identity.Value.Apply();
            Result = _framebuffer1;
            return true;
        }

        var edits = _edits.ToList();

        PixelBufferModel pbDest = _pixelBuffer1!, pbSrc = _pixelBuffer2!;
        FrameBufferModel fbDest = _framebuffer1!, fbSrc = _framebuffer2!;
        IEditModel?  currentEdit = edits.First(), lastEdit = null;

        SwapBuffers();

        // Initial pass: load source texture.
        if (!currentEdit.Apply())
        {
            _logger.LogError("Failed to apply first edit during rendering pass. (Faulty edit: {Edit})", currentEdit);
            return false;
        }

        lastEdit = edits.First();

        // Subsequent passes: switch between frame buffers.
        foreach (var edit in edits[1..])
        {
            currentEdit = edit;

            SwapBuffers();

            if (!edit.Apply())
            {
                _logger.LogError("Failed to apply edit during rendering pass. (Faulty edit: {Edit})", edit);
                return false;
            }

            lastEdit = edit;
            currentEdit = null;
        }

        if (lastEdit is IEditModel<TextureModel, FrameBufferModel> lastEditFrameBuffer)
        {
            Result = lastEditFrameBuffer.Destination;
        }
        else if (lastEdit is IEditModel<PixelBufferModel, PixelBufferModel> lastEditPixelBuffer)
        {
            Result = ConvertToFb(lastEditPixelBuffer.Destination, fbDest);
        }

        return true;

        FrameBufferModel ConvertToFb(PixelBufferModel from, FrameBufferModel to)
        {
            to.Data.LoadData(from.Width, from.Height, from.Data);
            return to;
        }

        PixelBufferModel ConvertToPb(TextureModel from, PixelBufferModel to)
        {
            to.LoadData(from);
            return to;
        }

        void SwapBuffers()
        {
            (pbDest, pbSrc) = (pbSrc, pbDest);
            (fbDest, fbSrc) = (fbSrc, fbDest);

            switch (currentEdit)
            {
                case IEditModel<TextureModel, FrameBufferModel> currentFb:
                    currentFb.Destination = fbDest;
                    currentFb.Sources = lastEdit switch
                    {
                        null => [_sourceTexture],
                        IEditModel<PixelBufferModel, PixelBufferModel> lastPb => [ConvertToFb(lastPb.Destination, fbSrc).Data],
                        IEditModel<TextureModel, FrameBufferModel> lastFb => [lastFb.Destination.Data],
                        _ => throw new NotSupportedException()
                    };
                    break;
                case IEditModel<PixelBufferModel, PixelBufferModel> currentPb:
                    currentPb.Destination = pbDest;
                    currentPb.Sources = lastEdit switch
                    {
                        null => [ConvertToPb(_sourceTexture, pbSrc)], 
                        IEditModel<PixelBufferModel, PixelBufferModel> lastPb => [lastPb.Destination],
                        IEditModel<TextureModel, FrameBufferModel> lastFb => [ConvertToPb(lastFb.Destination.Data, pbSrc)],
                        _ => throw new NotSupportedException()
                    };
                    break;
            }
        }
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

    private bool _disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            _pixelBuffer1?.Dispose();
            _pixelBuffer2?.Dispose();
            _framebuffer1?.Dispose();
            _framebuffer2?.Dispose();
            _sourceTexture?.Dispose();
            if (_identity.IsValueCreated) _identity.Value.Dispose();

            foreach (var edit in _edits)
            {
                if (edit is IDisposable disposable)
                    disposable.Dispose();
            }

            _disposedValue = true;
        }
    }

    ~StackEditorViewModel()
    {
        // https://www.khronos.org/opengl/wiki/Common_Mistakes#The_Object_Oriented_Language_Problem
        // The OpenGL resources have to be released from a thread with an active OpenGL Context.
        // The GC runs on a separate thread, thus releasing unmanaged GL resources inside the finalizer
        // is not possible.
        if (_disposedValue == false)
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