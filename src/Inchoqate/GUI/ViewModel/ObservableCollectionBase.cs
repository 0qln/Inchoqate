using System.Collections.ObjectModel;

namespace Inchoqate.GUI.ViewModel
{
    public class ObservableCollectionBase<T> : ObservableCollection<T>, IMoveItemsWrapper
    {
        public ObservableCollectionBase() : base()
        {
        }

        public ObservableCollectionBase(IEnumerable<T> @enum) : base(@enum)
        {
        }

        public ObservableCollectionBase(List<T> list) : base(list)
        {
        }
    }
}
