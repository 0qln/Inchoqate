using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel;

public class EditorNodeCollectionLinear(EventTreeViewModel relayTarget)
    : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget)
{
}