using Inchoqate.GUI.Model;

namespace Inchoqate.GUI.ViewModel;

public class EditorNodeCollectionDynamic(EventTreeViewModel relayTarget)
    : MonitoredObservableItemCollection<EditBaseDynamic>(relayTarget)
{
}