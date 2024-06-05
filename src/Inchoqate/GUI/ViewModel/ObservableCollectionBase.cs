using System.Collections.ObjectModel;

namespace Inchoqate.GUI.ViewModel
{
    public class ObservableCollectionBase<T> : ObservableCollection<T>, IMoveItemsWrapper { }
}
