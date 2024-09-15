using Inchoqate.GUI.Model;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel;

public abstract class EditBaseViewModel : BaseViewModel, IEditModel
{
    [JsonIgnore]
    public abstract ObservableCollection<ContentControl> OptionControls { get; }

    public abstract int ExpectedInputCount { get; }

    public abstract bool Apply();

    public override string ToString()
    {
        return Title ?? GetType().Name;
    }
}