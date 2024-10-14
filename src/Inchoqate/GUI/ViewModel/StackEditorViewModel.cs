using Inchoqate.GUI.Model;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.ViewModel.Events;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;

namespace Inchoqate.GUI.ViewModel;

public class StackEditorViewModel : RenderEditorViewModel, IDisposable
{
    private static readonly ILogger Logger = FileLoggerFactory.CreateLogger<StackEditorViewModel>();

    private FrameBuffer? _framebuffer1, _framebuffer2;
    private PixelBuffer? _pixelBuffer1, _pixelBuffer2;
    private Texture? _sourceTexture;
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
                Invalidate();
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
    private void ReloadBuffer(ref FrameBuffer? buffer)
    {
        buffer?.Dispose();
        buffer = new((int)RenderSize.Width, (int)RenderSize.Height, out var success);
        if (!success)
        {
            Logger.LogError("Failed to create framebuffer.");
        }
    }

    private void ReloadBuffer(ref PixelBuffer? buffer)
    {
        buffer?.Dispose();
        buffer = new(
            (int)RenderSize.Width * (int)RenderSize.Height * Texture.PixelDepth,
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
            Result!.Data.BorderColor = VoidColor;
            return true;
        }

        var edits = _edits.ToList();

        PixelBuffer pbDest = _pixelBuffer1!, pbSrc = _pixelBuffer2!;
        FrameBuffer fbDest = _framebuffer1!, fbSrc = _framebuffer2!;
        IEdit?  currentEdit = edits.First(), lastEdit = null;

        SwapBuffers();

        // Initial pass: load source texture.
        if (!currentEdit.Apply())
        {
            Logger.LogError("Failed to apply first edit during rendering pass. (Faulty edit: {Edit})", currentEdit);
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
                Logger.LogError("Failed to apply edit during rendering pass. (Faulty edit: {Edit})", edit);
                return false;
            }

            lastEdit = edit;
        }

        if (lastEdit is IEdit<Texture, FrameBuffer> lastEditFrameBuffer)
        {
            Result = lastEditFrameBuffer.Destination;
        }
        else if (lastEdit is IEdit<PixelBuffer, PixelBuffer> lastEditPixelBuffer)
        {
            Result = ConvertToFb(lastEditPixelBuffer.Destination, fbDest);
        }
        else
        {
            throw new NotSupportedException();
        }

        Result.Data.BorderColor = VoidColor;

        return true;

        FrameBuffer ConvertToFb(PixelBuffer from, FrameBuffer to)
        {
            to.Data.LoadData(from.Width, from.Height, from.Data);
            return to;
        }

        PixelBuffer ConvertToPb(Texture from, PixelBuffer to)
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
                case IEdit<Texture, FrameBuffer> currentFb:
                    currentFb.Destination = fbDest;
                    currentFb.Sources = lastEdit switch
                    {
                        null => [_sourceTexture],
                        IEdit<PixelBuffer, PixelBuffer> lastPb => [ConvertToFb(lastPb.Destination, fbSrc).Data],
                        IEdit<Texture, FrameBuffer> lastFb => [lastFb.Destination.Data],
                        _ => throw new NotSupportedException()
                    };
                    break;
                case IEdit<PixelBuffer, PixelBuffer> currentPb:
                    currentPb.Destination = pbDest;
                    currentPb.Sources = lastEdit switch
                    {
                        null => [ConvertToPb(_sourceTexture, pbSrc)], 
                        IEdit<PixelBuffer, PixelBuffer> lastPb => [lastPb.Destination],
                        IEdit<Texture, FrameBuffer> lastFb => [ConvertToPb(lastFb.Destination.Data, pbSrc)],
                        _ => throw new NotSupportedException()
                    };
                    break;
            }
        }
    }


    public override void SetSource(Texture? value)
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
            Logger.LogWarning("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}