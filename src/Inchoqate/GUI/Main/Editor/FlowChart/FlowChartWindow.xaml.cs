using Inchoqate.GUI.Titlebar;
using Microsoft.Extensions.Logging;
using Miscellaneous.Logging;
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
using System.Windows.Shapes;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    /// <summary>
    /// Interaction logic for FlowChartWindow.xaml
    /// </summary>
    public partial class FlowChartWindow : BorderlessWindow
    {
        private readonly ILogger<MainWindow> _logger = FileLoggerFactory.CreateLogger<MainWindow>();


        #region AddGrayscale Command

        public static readonly DependencyProperty AddGrayscaleNodeProperty = DependencyProperty.Register(
            "DP_AddGrayscaleNode", typeof(ICommand), typeof(FlowChartWindow));

        public static readonly RoutedCommand AddGrayscaleNodeCommand = new(
            "RC_AddGrayscaleNode", typeof(FlowChartWindow), [new KeyGesture(Key.N, ModifierKeys.Control)]);

        public void AddGrayscaleNode()
        {
            var newNode = new N_GrayScale();
            E_FlowChartEditor.E_MainCanvas.Children.Add(newNode);
            newNode.SetNext(E_FlowChartEditor.E_OutputNode);
        }

        #endregion


        public FlowChartWindow()
        {
            InitializeComponent();

            SetValue(AddGrayscaleNodeProperty, new ActionButtonCommand(AddGrayscaleNode));

            _logger.LogInformation("Flowchart window initiated.");
        }
    }
}
