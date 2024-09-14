using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
