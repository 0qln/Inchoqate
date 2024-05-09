
using Inchoqate.Miscellaneous;
using System.Windows;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    public class GrayScale : Node, INode
    {
        public bool RequiresBreak => false;

        public List<INode> Next
        {
            get
            {
                return null;
            }
        }

        public List<INode> Prev
        {
            get
            {
                return null;
            }
        }


        public static readonly DependencyProperty FilterOpacityProperty = DependencyProperty.Register(
            "FilterOpacity", typeof(double), typeof(GrayScale));

        public double FilterOpacity
        {
            get => (double)GetValue(FilterOpacityProperty);
            set => SetValue(FilterOpacityProperty, value);
        }


        public GrayScale()
        {
        }
    }
}
