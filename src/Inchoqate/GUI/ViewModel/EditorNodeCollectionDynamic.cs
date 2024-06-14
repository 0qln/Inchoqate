using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionDynamic(IEventTreeHost relayTarget)
        : MonitoredObservableItemCollection<EditBaseDynamic>(relayTarget)
    {
    }
}
