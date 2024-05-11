
using Inchoqate.Miscellaneous;
using System.Windows;
using System.Windows.Controls;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    public class N_GrayScale : Node, INode
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
            "FilterOpacity", typeof(double), typeof(N_GrayScale));

        public double FilterOpacity
        {
            get => (double)GetValue(FilterOpacityProperty);
            set => SetValue(FilterOpacityProperty, value);
        }


        public N_GrayScale()
        {
            base.Title = "Grayscale";
            base.Options =
            [
                new Slider(),
                new Button() { Content="Button" }
            ];
        }
    }
}
