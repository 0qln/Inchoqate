using CommunityToolkit.Mvvm.ComponentModel;

namespace Inchoqate.GUI.ViewModel
{
    public class ObservableStruct<T>(T value) : ObservableObject
        where T : struct
    {
        private T _value = value;

        public T Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }

}
