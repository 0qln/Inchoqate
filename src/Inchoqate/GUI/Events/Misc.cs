namespace Inchoqate.GUI.Events
{
    public class EventEventArgsBase : EventArgs
    {
        public required Event Event { get; init; }
    }


    public class NotifyEventOcccuredEventArgs : EventEventArgsBase { }

    public delegate void NotifyEventOccuredEventHandler(IEventRelay? sender, NotifyEventOcccuredEventArgs e);


    public class NotifyEventRevertedEventArgs : EventEventArgsBase { }

    public delegate void NotifyEventRevertedEventHandler(IEventRelay? sender, NotifyEventRevertedEventArgs e);


    public class NotifyEventRelayedEventArgs : EventEventArgsBase
    {
        public required IEventHost RelayTarget { get; init; }
    }

    public delegate void NotifyEventRelayedEventHandler(IEventRelay? sender, NotifyEventRelayedEventArgs e);


}
