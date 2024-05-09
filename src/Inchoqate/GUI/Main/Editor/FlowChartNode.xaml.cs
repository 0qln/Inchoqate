using Inchoqate.GUI.Titlebar;
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

namespace Inchoqate.GUI.Main.Editor
{
    public partial class OptionCollection : ObservableCollection<Control>
    {
    }


    /// <summary>
    /// Interaction logic for FlowChartNode.xaml
    /// </summary>
    public partial class FlowChartNode : UserControl
    {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(FlowChartNode));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty OptionsProperty = DependencyProperty.Register(
            "Options", typeof(OptionCollection), typeof(FlowChartNode));

        public OptionCollection Options
        {
            get => (OptionCollection)GetValue(OptionsProperty);
            set => SetValue(OptionsProperty, value);
        }


        public FlowChartNode()
        {
            InitializeComponent();
        }
    }
}
