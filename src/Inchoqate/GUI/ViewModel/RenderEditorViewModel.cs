using CommunityToolkit.Mvvm.ComponentModel;
using Inchoqate.GUI.Model;
using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI.ViewModel;

public abstract class RenderEditorViewModel : BaseViewModel, IEditorModel<TextureModel, FrameBufferModel>
{
    protected bool _computed;

    protected FrameBufferModel? _result;

    protected Size _renderSize, _sourceSize;

    private Color _voidColor;


    public abstract IEventRelayModel<EventViewModelBase>? Edits { get; }

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

    public FrameBufferModel? Result
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

    public abstract void SetSource(TextureModel? source);
}