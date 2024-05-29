using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Resources;
using System.IO;

namespace Inchoqate.GUI.Windows
{
    public partial class MainWindow : BorderlessWindowBase
    {
        private FlowchartEditorWindow? _editorWindow;


        public static readonly RoutedCommand OpenFlowchartEditorCommand =
            new RoutedCommand(
                "OpenFlowchartEditor",
                typeof(MainWindow),
                [new KeyGesture(Key.E, ModifierKeys.Control)]);


        public MainWindow()
        {
            InitializeComponent();

            PreviewImage.ImageSource = @"C:\Users\User\OneDrive\Bilder\Wallpapers\z\wallhaven-l8rloq.jpg";

            Closed += delegate
            {
                Application.Current.Shutdown();
            };
        }

        private void SliderThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (e.HorizontalChange != 0)
            {
                Sidebar.Width = Math.Clamp(Sidebar.Width - e.HorizontalChange, 0, ActualWidth - SliderThumb.ActualWidth);
            }
        }

        private void OpenFlowchartEditorCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_editorWindow is not null)
            {
                return;
            }

            _editorWindow = new FlowchartEditorWindow();
            _editorWindow.Show();
            _editorWindow.Closed += delegate
            {
                _editorWindow = null;
            };
        }
    }
}