
using Inchoqate.Miscellaneous;
using System.Windows;

namespace Inchoqate.GUI.Main.Editor
{
    public class FCN_GrayScale : FlowChartNode, IFlowChartNode
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


        public FCN_GrayScale()
        {
        }
    }
}
