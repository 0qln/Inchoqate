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
        /// The first event.
        /// </summary>
        private Event? _initialEvent;

        /// <summary>
        /// The most recent event.
        /// </summary>
        private Event? _current;


        /// <summary>
        /// Add an event to the event stack.
        /// </summary>
        /// <param name="e"></param>
        public void Push(Event e)
        {
            if (_current is null)
            {
                Debug.Assert(_initialEvent is null);
                _initialEvent = e;
                _current = e;
            }
            else
            {
                _current.Next.Add(e);
                e.Previous = _current;
                _current = e;
            }
        }

        /// <summary>
        /// Undo the most recent event.
        /// </summary>
        public void Undo()
        {
            if (_current != _initialEvent && _current is not null)
            {
                _current.Undo();
                _current = _current.Previous;
            }
        }

        /// <summary>
        /// Redo the next event.
        /// </summary>
        /// <param name="next">The index of the event to redo.</param>
        public void Redo(int next = 0)
        {
            if (_current is not null && _current.Next.Count > next)
            {
                _current = _current.Next[next];
                _current.Do();
            }
        }
    }
}
