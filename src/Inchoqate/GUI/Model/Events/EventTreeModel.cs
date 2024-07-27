namespace Inchoqate.GUI.Model.Events
{
    public interface IEventTree
    {
        /// <summary>
        /// The first event.
        /// </summary>
        IEvent Initial { get; }

        /// <summary>
        /// The current event.
        /// </summary>
        IEvent Current { get; }

        /// <summary>
        /// Add an event to the event stack.
        /// </summary>
        /// <param name="e"></param>
        bool Novelty(IEvent e);

        /// <summary>
        /// Redo the next event.
        /// </summary>
        /// <param name="next">The index of the event to redo. Defaults to the most recent event.</param>
        bool Redo(int next = 0);

        /// <summary>
        /// Undo the most recent event.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        bool Undo();
    }

    public class EventTreeModel : IEventTree
    {
        /// <summary>
        /// Dummy event.
        /// </summary>
        protected sealed class DummyEvent : EventModel
        {
            public override void Do() { }
            public override void Undo() { }

            public override string ToString()
            {
                return "Initial Event";
            }
        }

        /// <summary>
        /// Used to lock the manager from changes that originate in apply/revert actions.
        /// </summary>
        private volatile bool _locked = false;
        private readonly EventModel _initialEvent = new DummyEvent();
        private IEvent _current;

        /// <summary>The initial event (dummy).</summary>
        public IEvent Initial => _initialEvent;

        public IEvent Current => _current;


        public EventTreeModel()
        {
            _current = _initialEvent;
        }


        public bool Novelty(IEvent e)
        {
            if (_locked || _current.Next.ContainsKey(e.CreationDate))
                return false;

            _current.Next.Add(e.CreationDate, e);
            e.Previous = _current;
            _current = e;
            return true;
        }

        public bool Undo()
        {
            if (_locked || _current == _initialEvent)
                return false;

            // could modify state of the application and
            // allow for an event to be tried to push
            _locked = true; // lock
            _current.Undo();
            _locked = false; // unlock
            _current = _current.Previous!;
            return true;
        }

        public bool Redo(int next = 0)
        {
            if (_locked || next >= _current.Next.Count)
                return false;

            _current = _current.Next.Values[next];

            // could modify state of the application and
            // allow for an event to be tried to push
            _locked = true; // lock
            _current.Do();
            _locked = false; // unlock

            return true;
        }
    }
}
