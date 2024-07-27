using Inchoqate.GUI.Model.Events;
using MvvmHelpers;
using System.Collections.ObjectModel;

namespace Inchoqate.GUI.ViewModel
{
    public class EventTreeViewModel : BaseViewModel, IEventTree
    {
        private readonly EventTreeModel _model;

        /// <summary>
        /// All registered event trees.
        /// </summary>
        public static ObservableCollection<EventTreeViewModel> RegisteredTrees { get; private set; } = [];

        public IEvent Initial => _model.Initial;

        public IEvent Current => _model.Current;


        public EventTreeViewModel(EventTreeModel model, string title)
        {
            _model = model;
            Title = title;
            RegisteredTrees.Add(this);
        }


        public bool Novelty(IEvent e)
        {
            var result = ((IEventTree)_model).Novelty(e);
            if (result) OnPropertyChanged(nameof(Current));
            return result;
        }

        public bool Redo(int next = 0)
        {
            var result = ((IEventTree)_model).Redo(next);
            if (result) OnPropertyChanged(nameof(Current));
            return result;
        }

        public bool Undo()
        {
            var result = ((IEventTree)_model).Undo();
            if (result) OnPropertyChanged(nameof(Current));
            return result;
        }
    }
}
