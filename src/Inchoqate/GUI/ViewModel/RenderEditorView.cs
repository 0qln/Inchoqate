using System.Windows;
using System.Windows.Media;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel;

public abstract class RenderEditorViewModel : BaseViewModel, IEditor<Texture, FrameBuffer>
{
    protected bool _computed;

    protected FrameBuffer? _result;

    protected Size _renderSize, _sourceSize;

    private Color _voidColor;


    // TODO: this is a mistake. what events does it delegate?
    // public abstract IEventDelegate<EventViewModelBase>? Edits { get; }

    public abstract EventTreeViewModel EventTree { get; }

    public Color VoidColor
    {
        get => _voidColor;
        set => SetProperty(ref _voidColor, value);
    }

    public Size SourceSize
    {
        get => _sourceSize;
        protected set => SetProperty(ref _sourceSize, value);
    }

    public Size RenderSize
    {
        get => _renderSize;
        set => SetProperty(ref _renderSize, value);
    }

    public bool Computed
    {
        get => _computed;
        protected set => SetProperty(ref _computed, value);
    }

    public FrameBuffer? Result
    {
        get => _result;
        protected set => SetProperty(ref _result, value);
    }


    protected override void HandlePropertyChanged(string? propertyName)
    {
        switch (propertyName)
        {
            case nameof(Result):
                Computed = Result is not null;
                break;
            case nameof(RenderSize):
                Invalidate();
                break;
        }
    }


    public virtual void Invalidate()
    {
        Result = null;
    }

    public abstract bool Compute();

    public abstract void SetSource(Texture? source);
}