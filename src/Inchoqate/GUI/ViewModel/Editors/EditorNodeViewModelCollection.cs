using Inchoqate.GUI.ViewModel.Edits;
using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel.Editors;

public class EditorNodeViewModelCollection(EventTreeViewModel relayTarget)
    : MonitoredObservableItemCollection<EditBaseViewModel>(relayTarget);