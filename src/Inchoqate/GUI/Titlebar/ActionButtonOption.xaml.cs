using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Inchoqate.GUI.Titlebar
{
    /// <summary>
    /// Interaction logic for ActionButtonOption.xaml
    /// </summary>
    public partial class ActionButtonOption : UserControl
    {
        public static readonly DependencyProperty ShortcutProperty = DependencyProperty.Register(
            "Shortcut", typeof(KeyboardShortcut), typeof(ActionButtonOption));

        public KeyboardShortcut Shortcut
        {
            get => (KeyboardShortcut)GetValue(ShortcutProperty);
            set => SetValue(ShortcutProperty, value);
        }


        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(Image), typeof(ActionButtonOption));

        public Image Icon
        {
            get => (Image)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ActionButtonOption));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(ActionButtonOption));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }


        public ActionButtonOption()
        {
            InitializeComponent();
        }
    }
}
