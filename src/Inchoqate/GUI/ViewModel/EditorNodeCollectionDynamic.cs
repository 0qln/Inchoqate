using Inchoqate.GUI.Events;

namespace Inchoqate.GUI.ViewModel
{
    public class EditorNodeCollectionDynamic
        : MonitoredObservableItemCollection<EditBaseDynamic>
    {
        public EditorNodeCollectionDynamic(IEventHost relayTarget) : base(relayTarget)
        {
        }
    }
}
