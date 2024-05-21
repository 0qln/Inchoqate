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

namespace Inchoqate.GUI.Titlebar
{
    /// <summary>
    /// Interaction logic for ActionButtonAction.xaml
    /// </summary>
    public partial class ActionButtonAction : ActionButtonOption
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(ImageSource), typeof(ActionButtonAction));

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(ActionButtonAction));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public static readonly DependencyProperty IndicatorVisibilityProperty = DependencyProperty.Register(
            "IndicatorVisibility", typeof(Visibility), typeof(ActionButtonAction), new(Visibility.Collapsed));

        public Visibility IndicatorVisibility
        {
            get => (Visibility)GetValue(IndicatorVisibilityProperty);
            set => SetValue(IndicatorVisibilityProperty, value);
        }


        public static readonly DependencyProperty ShortcutProperty = DependencyProperty.Register(
            "Shortcut", typeof(CommandBinding), typeof(ActionButtonAction), new(null, OnShortcutChanged));

        public CommandBinding Shortcut
        {
            get => (CommandBinding)GetValue(ShortcutProperty);
            set => SetValue(ShortcutProperty, value);
        }

        private static void OnShortcutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (ActionButtonAction)d;
            var shortcut = (CommandBinding)e.NewValue;
            shortcut.Executed += (_, args) => button.Command.Execute(args);
        }


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(ActionButtonAction));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }


        //double IActionButtonOption.Col_Icon_MinWidth
        //{
        //    get => Col_Icon.MinWidth;
        //    set => Col_Icon.MinWidth = value;
        //}
        //double IActionButtonOption.Col_Title_MinWidth => Col_Title;
        //double IActionButtonOption.Col_Shortcut_MinWidth => Col_Shortcut;
        //double IActionButtonOption.Col_Indicator_MinWidth => Col_Indicator.MinWidth;


        public ActionButtonAction()
        {
            InitializeComponent();
        }
    }
}
