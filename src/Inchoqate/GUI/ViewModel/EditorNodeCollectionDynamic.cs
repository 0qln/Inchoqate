using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionDynamic(IEventHost relayTarget)
        : MonitoredObservableItemCollection<EditBaseDynamic>(relayTarget)
    {
    }
}
