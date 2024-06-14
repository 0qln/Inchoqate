using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionLinear(IEventTreeHost relayTarget)
        : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget)
    {
    }
}
