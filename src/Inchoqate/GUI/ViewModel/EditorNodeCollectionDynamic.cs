using Inchoqate.GUI.ViewModel.Events;

namespace Inchoqate.GUI.ViewModel;

public class EditorNodeCollectionDynamic(EventTreeViewModel relayTarget)
    : MonitoredObservableItemCollection<EditBaseDynamic>(relayTarget)
{
}