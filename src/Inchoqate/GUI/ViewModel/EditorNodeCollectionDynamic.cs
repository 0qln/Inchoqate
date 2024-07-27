using Inchoqate.GUI.Model.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionDynamic(EventTreeViewModel relayTarget)
        : MonitoredObservableItemCollection<EditBaseDynamic>(relayTarget)
    {
    }
}
