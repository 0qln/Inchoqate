using Inchoqate.GUI.Model;
using Inchoqate.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Specialized;
using Inchoqate.GUI.View;

namespace Inchoqate.GUI.ViewModel;

public class StackEditorViewModel : RenderEditorViewModel, IDisposable
{
    private static readonly ILogger _logger = FileLoggerFactory.CreateLogger<StackEditorViewModel>();

    private FrameBufferModel? _framebuffer1, _framebuffer2;
    private TextureModel? _sourceTexture;
    private readonly EditorNodeCollectionLinear _edits;


    public override EditorNodeCollectionLinear Edits => _edits;

    public sealed override EventTreeViewModel EventTree { get; }


    protected override void HandlePropertyChanged(string? propertyName)
    {
        base.HandlePropertyChanged(propertyName);

        switch (propertyName)
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
    }

    public StackEditorViewModel()
        : this(new("Stack Editor"))
    {
    }

    public StackEditorViewModel(EventTreeViewModel eventTree)
    {
        EventTree = eventTree;
        _edits = new(relayTarget: EventTree);
        foreach (var @event in EventTree)
        {
            if (@event is IParameterInjected<ICollection<EditBaseLinear>> pi)
                pi.Parameter = _edits;

            @event.Do();
        }
        _edits.CollectionChanged += (_, _) => Invalidate();
        _edits.ItemsPropertyChanged += (_, _) => Invalidate();
    }


    // TODO: if the new size is smaller, don't dispose and just use a subset of the buffer.
    private void ReloadBuffer(ref FrameBufferModel? buffer)
    {
        buffer?.Dispose();
        buffer = new((int)_renderSize.Width, (int)_renderSize.Height, out var success);
        if (!success)
        {
            _logger.LogError("Failed to create framebuffer.");
            return;
        }

        buffer.Data.BorderColor = VoidColor;
    }


    public override bool Compute()
    {
        if (_sourceTexture is null)
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

        // Subsequent passes: switch between frame buffers.
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
            _framebuffer1?.Dispose();
            _framebuffer1?.Dispose();
            _sourceTexture?.Dispose();

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