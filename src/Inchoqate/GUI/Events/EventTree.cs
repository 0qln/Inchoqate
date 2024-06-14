﻿using System.Collections.ObjectModel;

namespace Inchoqate.GUI.Events
{
    public class EventTree
    {
        /// <summary>
        /// Dummy event.
        /// </summary>
        protected sealed partial class DummyEvent : Event
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

        /// <summary>
        /// The first event.
        /// </summary>
        private readonly Event _initialEvent = new DummyEvent();

        /// <summary>
        /// The most recent event.
        /// </summary>
        private Event _current;

        /// <summary>
        /// The name of the event tree.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// All registered event trees.
        /// </summary>
        public static ObservableCollection<EventTree> RegisteredTrees { get; private set; } = [];

        public Event InitialEvent => _initialEvent;


        public EventTree()
        {
            _current = _initialEvent;
            RegisteredTrees.Add(this);
        }

        static EventTree()
        {
        }


        /// <summary>
        /// Add an event to the event stack.
        /// </summary>
        /// <param name="e"></param>
        public void Novelty(Event e)
        {
            if (_locked || _current.Next.ContainsKey(e.CreationDate))
                return;

            _current.Next.Add(e.CreationDate, e);
            e.Previous = _current;
            _current = e;
        }

        /// <summary>
        /// Undo the most recent event.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void Undo()
        {
            if (_locked || _current == _initialEvent)
                return;

            // could modify state of the application and
            // allow for an event to be tried to push
            _locked = true; // lock
            _current.Undo();
            _locked = false; // unlock
            _current = _current.Previous!;
        }

        /// <summary>
        /// Redo the next event.
        /// </summary>
        /// <param name="next">The index of the event to redo. Defaults to the most recent event.</param>
        public void Redo(int next = 0)
        {
            if (_locked || next >= _current.Next.Count)
                return;

            _current = _current.Next.Values[next];

            // could modify state of the application and
            // allow for an event to be tried to push
            _locked = true; // lock
            _current.Do();
            _locked = false; // unlock
        }
    }
}
