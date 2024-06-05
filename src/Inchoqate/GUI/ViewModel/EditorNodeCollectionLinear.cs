using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionLinear
        : MonitoredObservableItemCollection<EditBaseLinear>
    {
        public EditorNodeCollectionLinear(IEventHost relayTarget) : base(relayTarget)
        {
        }
    }
}
