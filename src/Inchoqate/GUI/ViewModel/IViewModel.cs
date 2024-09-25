using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.ViewModel
{
    interface IViewModel<out TModel>
    {
        public TModel Model { get; }
    }
}
