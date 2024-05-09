using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
using OpenTK.Wpf;
using System.Diagnostics;
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
using System.IO;
using OpenTK.Compute.OpenCL;
using Inchoqate.Miscellaneous;
using Utillities.Wpf;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Shell;
using System.Globalization;
using Inchoqate.GUI.Titlebar;
using System.Collections.ObjectModel;
using Inchoqate.GUI.Main.Editor.FlowChart;

namespace Inchoqate.GUI
{
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty WrappingProperty = DependencyProperty.Register(
            "Wrapping", typeof(WindowWrapping), typeof(MainWindow));

        public WindowWrapping Wrapping
        {
            get => (WindowWrapping)GetValue(WrappingProperty); 
            set => SetValue(WrappingProperty, value);
        }


        private readonly ILogger<MainWindow> _logger = FileLoggerFactory.CreateLogger<MainWindow>();


        public MainWindow()
        {
            BuildFiles.Initiate(clearOldData: true);

            InitializeComponent();

            // Enables rounded corners for the borderless window.
            // TODO: When maximized, the corners are still rounded.
            Wrapping = WindowWrapper.Wrap(this);

            E_Titlebar.Windowed += delegate
            {
                if (WindowState == WindowState.Normal)
                {
                    Wrapping.CornerRadius = 15;
                }
                else
                {
                    Wrapping.CornerRadius = 0;
                }
            };

            _logger.LogInformation("Main window initiated.");
        }


        #region Seperator logic

        public bool Seperator_IsDragging { get; private set; }
        private Point Mouse_Position;

        private void Seperator_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Seperator_IsDragging = true;
        }

        private void Seperator_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Seperator_IsDragging = false;
        }

        private void Seperator_Dragging(object sender, MouseEventArgs e)
        {
            var nextPosition = e.GetPosition(this);

            if (Seperator_IsDragging)
            {
                var mouseDelta = nextPosition - Mouse_Position;
                if (EditorInputs.Width >= mouseDelta.X)
                    EditorInputs.Width -= mouseDelta.X;
            }
        }

        #endregion


        private void This_MouseMove(object sender, MouseEventArgs e)
        {
            Seperator_Dragging(sender, e);

            Mouse_Position = e.GetPosition(this);
        }

        private void This_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Seperator_MouseUp(sender, e);
        }
    }
}