using System.Collections.ObjectModel;
using System.Windows.Controls;
using Inchoqate.GUI.Model.Graphics;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel;

public abstract class EditBaseViewModel : BaseViewModel, IEdit
{
    [JsonIgnore]
    public abstract OptionControls OptionControls { get; }

    public abstract int ExpectedInputCount { get; }

    public abstract bool Apply();

    public override string ToString()
    {
        return Title ?? GetType().Name;
    }
}

public class OptionControls : ObservableCollection<OptionControl>;

public record OptionControl(Control Control, string Title);