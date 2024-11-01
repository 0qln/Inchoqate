using System.Windows;
using System.Windows.Media;
using Inchoqate.GUI.Model.Events;
using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel.Editors;


public abstract class RenderEditorViewModel : BaseViewModel, IEditor<Texture, FrameBuffer>,
    IEventDelegate<RenderEditorSourceChangedEvent, RenderEditorViewModel>
{
    private bool _computed;

    private Size _renderSize, _sourceSize;

    private FrameBuffer? _result;

    private Color _voidColor;

    protected RenderEditorViewModel(EventTreeViewModel eventTree)
    {
        DelegationTarget = eventTree;
        Edits = new(eventTree);
        Edits.CollectionChanged += (_, _) => Invalidate();
        Edits.ItemsPropertyChanged += (_, _) => Invalidate();
    }

    public EditorNodeViewModelCollection Edits { get; }

    public Size SourceSize
    {
        get => _sourceSize;
        protected set => SetProperty(ref _sourceSize, value);
    }

    public Color VoidColor
    {
        get => _voidColor;
        set => SetProperty(ref _voidColor, value);
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


    public virtual void Invalidate()
    {
        Result = null;
    }

    public abstract bool Compute();

    public abstract void SetSource(Texture? source);

    public abstract Uri? GetUriSource();


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

    /// <inheritdoc />
    public IEventReceiver? DelegationTarget { get; set; }
}