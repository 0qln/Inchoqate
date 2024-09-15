namespace Inchoqate.GUI.Model;

public interface IEventTree<in TEvent>
    where TEvent : IEvent
{
    public bool Novelty(TEvent e, bool execute = false);
}