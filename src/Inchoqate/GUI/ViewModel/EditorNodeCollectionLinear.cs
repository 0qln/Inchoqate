using Inchoqate.GUI.Model.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionLinear(EventTreeViewModel relayTarget)
        : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget)
    {
    }
}
