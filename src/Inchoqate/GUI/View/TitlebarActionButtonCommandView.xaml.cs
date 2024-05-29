using System;
using System.Collections.Generic;
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

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for TitlebarActionButtonCommandView.xaml
    /// </summary>
    public partial class TitlebarActionButtonCommandView : TitlebarActionButtonOptionView
    {
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register(
                "Command", 
                typeof(ICommand), 
                typeof(TitlebarActionButtonCommandView));


        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }


        public TitlebarActionButtonCommandView()
        {
            InitializeComponent();
        }
    }
}
