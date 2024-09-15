using System.Collections.ObjectModel;
using Inchoqate.GUI.Model;
using Newtonsoft.Json;

namespace Inchoqate.GUI.ViewModel.Events;

public class LinearEditAddedEvent : ItemAddedEvent<EditBaseLinear>, IDeserializable<LinearEditAddedEvent>
{
}
