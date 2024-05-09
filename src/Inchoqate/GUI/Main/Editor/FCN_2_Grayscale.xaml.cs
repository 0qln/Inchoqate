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
    /// Interaction logic for FCN_2_Grayscale.xaml
    /// </summary>
    public partial class FCN_2_Grayscale : FCN_Base, IFlowChartNode
    {
        public bool RequiresBreak => false;

        public List<IFlowChartNode> Next
        {
            get
            {
                return null;
            }
        }

        public List<IFlowChartNode> Prev
        {
            get
            {
                return null;
            }
        }


        public static readonly DependencyProperty FilterOpacityProperty = DependencyProperty.Register(
            "FilterOpacity", typeof(double), typeof(FCN_GrayScale));

        public double FilterOpacity
        {
            get => (double)GetValue(FilterOpacityProperty);
            set => SetValue(FilterOpacityProperty, value);
        }


        public FCN_2_Grayscale()
        {
            InitializeComponent();
        }
    }
}
