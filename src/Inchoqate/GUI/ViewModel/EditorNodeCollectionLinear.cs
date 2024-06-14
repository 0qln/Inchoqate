using Inchoqate.GUI.Model.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionLinear(EventTreeModel relayTarget)
        : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget)
    {
    }
}
