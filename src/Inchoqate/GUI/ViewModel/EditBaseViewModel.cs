using Inchoqate.GUI.Model;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace Inchoqate.GUI.ViewModel
{
    public abstract class EditBaseViewModel : BaseViewModel, IEditModel
    {
        public abstract ObservableCollection<ContentControl> OptionControls { get; }

        public abstract int ExpectedInputCount { get; }

        public abstract bool Apply(IEditDestinationModel destination, params IEditSourceModel[] sources);

        public override string ToString()
        {
            return Title ?? GetType().Name;
        }
    }
}
