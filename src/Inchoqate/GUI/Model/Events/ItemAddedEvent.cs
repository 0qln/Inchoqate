using Inchoqate.GUI.Model.Graphics;
using Inchoqate.GUI.View;
using Inchoqate.GUI.ViewModel.Editors;

namespace Inchoqate.GUI.Model.Events;

public abstract class ItemAddedEvent<T> : CollectionEvent<T>
{
    /// <summary>
    /// The item to add.
    /// </summary>
    [ViewProperty]
    public virtual T? Item { get; init; }

    protected override bool InnerDo()
    {
        if (Dependency is null || Item is null)
            return false;

        Dependency.Add(Item);
        return true;
    }

    protected override bool InnerUndo()
    {
        if (Dependency is null || Item is null)
            return false;
        
        return Dependency.Remove(Item);
    }
}


public interface IRenderEditorProperty : IProperty
{
    public RenderEditorViewModel? RenderEditor { get; set; }
}

public class ActiveEditorChangedEvent : PropertyChangedEvent<IRenderEditorProperty, RenderEditorViewModel>
{
    /// <inheritdoc />
    protected override bool Setter(IRenderEditorProperty prop, RenderEditorViewModel? val)
    {
        prop.RenderEditor = val;
        return true;
    }
}

public class RenderEditorSourceChangedEvent : PropertyChangedEvent<RenderEditorViewModel, Uri>
{
    /// <inheritdoc />
    protected override bool Setter(RenderEditorViewModel prop, Uri? val)
    {
        var texture = val is null ? null : Texture.FromUri(val);
        prop.SetSource(texture);
        return true;
    }
}
