using System.Collections.ObjectModel;

namespace Inchoqate.GUI.View.MenuButton;

public class MenuButtonItemCollection(MenuButton parent) : ObservableCollection<MenuButtonItem>
{
    /// <inheritdoc />
    protected override void InsertItem(int index, MenuButtonItem item)
    {
        base.InsertItem(index, item);
        item.Parent = parent;
    }

    /// <inheritdoc />
    protected override void RemoveItem(int index)
    {
        var item = this[index];
        base.RemoveItem(index);
        item.Parent = null;
    }
}