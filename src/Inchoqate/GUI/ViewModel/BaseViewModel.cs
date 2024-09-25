using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.ViewModel
{
    public class BaseViewModel : MvvmHelpers.BaseViewModel 
    {
        protected virtual void HandlePropertyChanged(string? propertyName)
        {
        }

        public BaseViewModel()
        {
            PropertyChanged += (_, e) => HandlePropertyChanged(e.PropertyName);
        }

        #region PropertySetterOverloads


        protected bool SetProperty<T>(
            Func<T> backingStorePropertyReferenceGetter, T value, 
            [CallerMemberName] string propertyName = "", 
            Action? onChanged = null,
            Func<T, T, bool>? validateValue = null)
            where T : class
        {
            var backingStore = backingStorePropertyReferenceGetter();
            return base.SetProperty(ref backingStore, value, propertyName, onChanged, validateValue);
        }

        protected bool SetProperty<T>(
            Action<T> backingStoreSetter, T value, 
            [CallerMemberName] string propertyName = "", 
            Action? onChanged = null,
            Func<T, T, bool>? validateValue = null)
            where T : struct
        {
            T temp = default;
            var result = base.SetProperty(ref temp, value, propertyName, onChanged, validateValue);
            backingStoreSetter(temp);
            return result;
        }

        protected bool SetProperty<T>(
            Action<T?> backingStoreSetter, T? value, 
            [CallerMemberName] string propertyName = "", 
            Action? onChanged = null,
            Func<T?, T?, bool>? validateValue = null)
            where T : struct
        {
            T? temp = default;
            var result = base.SetProperty<T?>(ref temp, value, propertyName, onChanged, validateValue);
            backingStoreSetter(temp);
            return result;
        }
        

        #endregion
    }
}
