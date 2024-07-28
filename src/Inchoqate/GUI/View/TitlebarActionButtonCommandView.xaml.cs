using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Inchoqate.GUI.View
{
    /// <summary>
    /// Interaction logic for TitlebarActionButtonCommandView.xaml
    /// </summary>
    public partial class TitlebarActionButtonCommandView : TitlebarActionButtonOptionView
    {
        public static readonly DependencyProperty CommandProperty = 
            DependencyProperty.Register(
                nameof(Command), 
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

            SetBinding(CommandProperty, new Binding("Shortcut.Command") { Source = this });
        }
    }
}
