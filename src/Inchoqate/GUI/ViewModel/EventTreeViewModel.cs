using Inchoqate.GUI.Model;
using MvvmHelpers;
using System.Collections.ObjectModel;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Inchoqate.GUI.ViewModel;

/// <summary>
/// Provides a view model for an event tree model.
/// </summary>
public class EventTreeViewModel : BaseViewModel, IEventTree<EventViewModelBase>
{
    /// <summary>
    /// Dummy event.
    /// </summary>
    protected sealed class DummyEvent() : EventViewModelBase("Initial Event")
    {
        protected override bool InnerDo() => true;

        protected override bool InnerUndo() => true;
    }

    /// <summary>
    /// All registered event trees.
    /// </summary>
    public static ObservableCollection<EventTreeViewModel> RegisteredTrees { get; } = [];

    /// <summary>
    /// Used to lock the manager from changes that originate in apply/revert actions.
    /// </summary>
    private volatile bool _locked = false;
    private EventViewModelBase _current;

    public EventViewModelBase Initial { get; } = new DummyEvent();

    public EventViewModelBase Current
    {
        get => _current;
        private set
        {
            if (Equals(value, _current)) return;
            _current = value;
            OnPropertyChanged();
        }
    }


    public EventTreeViewModel(string title)
    {
        Title = title;
        _current = Initial;
        RegisteredTrees.Add(this);
    }


    public bool Novelty(EventViewModelBase e, bool execute = false)
    {
        if (_locked || !Current.Next.TryAdd(e.CreationDate, e))
            return false;

        if (execute) e.Do();

        e.Previous = Current;
        Current = e;
        return true;
    }

    public bool Undo()
    {
        if (_locked || Current == Initial || Current.Previous is null)
            return false;

        // could modify state of the application and
        // allow for an event to be tried to push
        _locked = true;
        Current.Undo();
        _locked = false;
        Current = Current.Previous;

        return true;
    }
        
    public bool Redo(int next = 0)
    {
        if (_locked || next >= Current.Next.Count)
            return false;

        _locked = true;
        var e = Current.Next.Values[next];
        e.Do();
        _locked = false;
        Current = e;

        return true;
    }
}