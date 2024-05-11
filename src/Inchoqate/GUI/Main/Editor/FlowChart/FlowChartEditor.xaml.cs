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

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    /// <summary>
    /// Interaction logic for EditorFlowChart.xaml
    /// </summary>
    public partial class FlowChartEditor : Page
    {
        public FlowChartEditor()
        {
            InitializeComponent();
        }

        // TODO: Lazy hotreload.
        // TODO: Compile non-multipass editnodes into a single shader.
        // TODO: output pipeline
        public void Compile() { }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            E_InputNode.SetNext(E_OutputNode);

            Canvas.SetTop(E_InputNode, 50);
            Canvas.SetLeft(E_InputNode, 50);

            // `Canvas.SetRight`/`Canvas.SetBottom` introduces glitches with the connection
            // adorner for some reason. Can't bother to figure out why...
            Canvas.SetTop(E_OutputNode, E_MainCanvas.ActualHeight - 50 - E_OutputNode.ActualHeight);
            Canvas.SetLeft(E_OutputNode, E_MainCanvas.ActualWidth - 50 - E_OutputNode.ActualWidth);
        }
    }
}
