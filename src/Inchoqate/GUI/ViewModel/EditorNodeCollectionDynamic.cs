using Inchoqate.GUI.Model.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionDynamic(EventTreeModel relayTarget)
        : MonitoredObservableItemCollection<EditBaseDynamic>(relayTarget)
    {
    }
}
