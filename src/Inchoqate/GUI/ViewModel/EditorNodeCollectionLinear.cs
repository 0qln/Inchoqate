using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel;

public class EditorNodeCollectionLinear(EventTreeViewModel relayTarget)
    : MonitoredObservableItemCollection<EditBaseLinear>(relayTarget)
{
}