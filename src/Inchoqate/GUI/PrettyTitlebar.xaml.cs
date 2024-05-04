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

namespace Inchoqate.GUI
{
    /// <summary>
    /// Interaction logic for PrettyTitlebar.xaml
    /// </summary>
    public partial class PrettyTitlebar : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title", typeof(string), typeof(PrettyTitlebar));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        private Window? _window;


        public PrettyTitlebar()
        {
            InitializeComponent();

            Loaded += (_,_) => _window = Window.GetWindow(this);
        }

        private void WindowedButton_Click(object sender, RoutedEventArgs e)
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
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_window is null)
            {
                return;
            }

            _window.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_window is null)
            {
                return;
            }

            _window.Close();
        }
    }
}
