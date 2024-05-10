using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
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
    public class ActionButtonCollection : ObservableCollection<ActionButton>
    {
        public ActionButtonCollection()
        {
        }
    }


    public partial class PrettyTitlebar : UserControl
    {
        private readonly ILogger<PrettyTitlebar> _logger = FileLoggerFactory.CreateLogger<PrettyTitlebar>();


        public static readonly DependencyProperty ActionButtonsProperty = DependencyProperty.Register(
            "ActionButtons", typeof(ActionButtonCollection), typeof(PrettyTitlebar));

        public ActionButtonCollection ActionButtons
        {
            get => (ActionButtonCollection)GetValue(ActionButtonsProperty);
            set => SetValue(ActionButtonsProperty, value);
        }


        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(PrettyTitlebar));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }


        public event EventHandler? Minizmized, Windowed, Closed;


        private Window? _window;


        public PrettyTitlebar()
        {
            InitializeComponent();

            Loaded += (_,_) => _window = Window.GetWindow(this);
        }

        private void E_WindowedButton_Click(object sender, RoutedEventArgs e)
        {
            if (_window is null)
            {
                return;
            }

            _window.WindowState = _window.WindowState switch
            {
                WindowState.Normal      => WindowState.Maximized,
                WindowState.Minimized   => WindowState.Normal,
                _                       => WindowState.Normal
            };

            Windowed?.Invoke(this, EventArgs.Empty);
        }

        private void E_MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_window is null)
            {
                return;
            }

            _window.WindowState = WindowState.Minimized;

            Minizmized?.Invoke(this, EventArgs.Empty);
        }

        private void E_ActionButtonStack_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (var button in ActionButtons)
            {
                // TODO: this logic will not work with keyboard navigation.

                if (// In this case the button toggles itself.
                    !button.IsMouseOver || 
                    // In this case the button does not toggle itself and we should collapse it.
                    button.E_ActionsCanvas.IsMouseOver)
                {
                    button.Collapse();
                }
            }
        }

        private void E_CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_window is null)
            {
                return;
            }

            _window.Close();

            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}
