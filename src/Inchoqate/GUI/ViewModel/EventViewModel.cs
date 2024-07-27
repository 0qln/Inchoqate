using System.Reflection;
using Inchoqate.GUI.Model.Events;
using MvvmHelpers;

namespace Inchoqate.GUI.ViewModel
{
    public class EventViewModel : BaseViewModel, IEvent
    {
        private readonly EventModel _model;

        public readonly Type ModelType;

        public DateTime CreationDate
        {
            get
            {
                return _model.CreationDate;
            }
        }

        public IEvent? Previous
        {
            get => _model.Previous;
            set
            {
                _model.Previous = value;
                OnPropertyChanged();

                // Implementing the idea in the comment below.
                if (value is EventViewModel viewModel)
                    viewModel.OnPropertyChanged(nameof(Next));
            }
        }

        // Although an 'ObservableSortedList' would be preferred, it is not 
        // necessary as an update in the 'Next' property of this object can be 
        // caught and acted upon by observing the 'Previous' property of the
        // next object.
        // => For ease of implementation we will just trust that the next ViewModel
        // will notify us of changes.
        public SortedList<DateTime, IEvent> Next
        {
            get => _model.Next;
        }

        public EventState State
        {
            get => _model.State;
        }

        public void Do()
        {
            _model.Do();
            OnPropertyChanged(nameof(State));
        }

        public void Undo()
        {
            _model.Undo();
            OnPropertyChanged(nameof(State));
        }

        public EventViewModel(EventModel model, string title) 
        {
            Title = title;
            _model = model;
            ModelType = _model.GetType();
        }

        public bool EqualsInner(EventViewModel other)
        {
            return _model.Equals(other._model);
        }

        public object? GetModelPropertyValue(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetValue(_model);
        }

        // /// <summary>
        // /// Replaces the model of this object with the given one.
        // /// </summary>
        // /// <param name="model"></param>
        // /// <returns></returns>
        // public static EventViewModel UpgradeModel(ref EventModel model)
        // {
        //
        // }
    }
}
