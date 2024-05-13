using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.Titlebar
{
    public class ActionButtonOptionCollection : ObservableCollection<ActionButtonOption>
    {
        public ActionButtonOptionCollection()
        {
        }
    }

    public class ActionButtonOption : UserControl
    {
        public static readonly DependencyProperty IconMinWidthProperty = DependencyProperty.Register(
            "IconMinWidth", typeof(double), typeof(ActionButtonOption));

        public double IconMinWidth
        {
            get => (double)GetValue(IconMinWidthProperty);
            set => SetValue(IconMinWidthProperty, value);
        }
    }
}
