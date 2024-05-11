using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Inchoqate.GUI.Titlebar
{
    public class ActionButtonOptionCollection : ObservableCollection<IActionButtonOption>
    {
        public ActionButtonOptionCollection()
        {
        }
    }


    public interface IActionButtonOption
    {
        public ColumnDefinition Col_Icon { get; }
        public ColumnDefinition Col_Title { get; }
        public ColumnDefinition Col_Shortcut { get; }
        public ColumnDefinition Col_Indicator { get; }
    }
}
