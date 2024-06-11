using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionLinear(IEventHost relayTarget)
        : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget)
    {
    }
}
