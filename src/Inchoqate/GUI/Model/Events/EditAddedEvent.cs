using Inchoqate.GUI.ViewModel.Edits;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.Model.Events;

public class EditAddedEvent : ItemAddedEvent<EditBaseViewModel>, IDeserializable<EditAddedEvent>;
