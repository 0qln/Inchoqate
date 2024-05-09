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

namespace Inchoqate.GUI.Main.Editor
{
    /// <summary>
    /// Interaction logic for EditorFlowChart.xaml
    /// </summary>
    public partial class EditorFlowChart : Page
    {
        public EditorFlowChart()
        {
            InitializeComponent();
        }

        // TODO: Lazy hotreload.
        // TODO: Compile non-multipass editnodes into a single shader.
        // TODO: output pipeline
        public void Compile() { }
    }
}
