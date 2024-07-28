using System.Collections.ObjectModel;

namespace Inchoqate.GUI.ViewModel.Events;

public class LinearEditAddedEvent(EditBaseLinear edit) 
    : ItemAddedEvent<EditBaseLinear>(edit, "Linear Edit Added")
{
}
