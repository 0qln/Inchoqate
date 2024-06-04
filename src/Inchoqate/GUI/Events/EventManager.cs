using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Events
{
    public class EventManager
    {
        /// <summary>
        /// Dummy event.
        /// </summary>
        protected sealed class DummyEvent : Event
        {
            public override void Do() { }
            public override void Undo() { }
        }


        /// <summary>
        /// The first event.
        /// </summary>
        private readonly Event _initialEvent = new DummyEvent();

        /// <summary>
        /// The most recent event.
        /// </summary>
        private Event _current;


        public EventManager()
        {
            _current = _initialEvent;
        }


        /// <summary>
        /// Add an event to the event stack.
        /// </summary>
        /// <param name="e"></param>
        public void Push(Event e)
        {
            _current.Next.Add(e);
            e.Previous = _current;
            _current = e;
        }

        /// <summary>
        /// Undo the most recent event.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void Undo()
        {
            if (_current == _initialEvent)
                throw new NullReferenceException("Cannot undo initial event.");

            _current.Undo();
            _current = _current.Previous!;
        }

        /// <summary>
        /// Redo the next event.
        /// </summary>
        /// <param name="next">The index of the event to redo.</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public void AdvanceAndRedo(int next = 0)
        {
            if (next >= _current.Next.Count) 
                throw new IndexOutOfRangeException();

            _current = _current.Next[next];
            _current.Do();
        }
    }
}
