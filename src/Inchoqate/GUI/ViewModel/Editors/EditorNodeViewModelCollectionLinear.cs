using Inchoqate.GUI.ViewModel.Edits;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel.Editors;

public class EditorNodeViewModelCollectionLinear(EventTreeViewModel relayTarget)
    : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget);