using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

public class LinearEditAddedEvent(EditBaseLinear edit) 
    : ItemAddedEvent<EditBaseLinear>(edit, "Linear Edit Added")
{

}
