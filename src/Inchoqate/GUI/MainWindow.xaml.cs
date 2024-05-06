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

namespace Inchoqate.GUI
{
    public class ActionButtonCollection : Titlebar.ActionButtonCollection
    {
        public ActionButtonCollection()
        {
            // This is null in xaml preview and will raise an annoying error.
            // TODO: relocate the resources, s.t. the preview can show the action buttons.
            if (Application.Current.MainWindow is not null)
            {
                var resources = Application.Current.MainWindow.Resources;
                Add((ActionButton)resources["K_ActionButton_File"]);
                Add((ActionButton)resources["K_ActionButton_View"]);
                Add((ActionButton)resources["K_ActionButton_Edit"]);
                Add((ActionButton)resources["K_ActionButton_Settings"]);
            }
        }
    }


    public partial class MainWindow : Window
    {


        private readonly ILogger<MainWindow> _logger = FileLoggerFactory.CreateLogger<MainWindow>();


        public MainWindow()
        {
            BuildFiles.Initiate(clearOldData: true);

            InitializeComponent();

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