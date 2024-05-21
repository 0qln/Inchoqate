
using Inchoqate.Miscellaneous;
using System.Windows;
using System.Windows.Controls;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    public class N_GrayScale : NodeModel, ISinglePassNode
    {
        public override bool RequiresBreak => false;


        private readonly List<NodeModel> _outputs = [];
        
        public override List<NodeModel> Next
        {
            get
            {
                return _outputs;
            }
        }


        private readonly List<NodeModel> _inputs = [];
        
        public override List<NodeModel> Prev
        {
            get
            {
                return _inputs;
            }
        }
         

        public static readonly DependencyProperty FilterOpacityProperty = DependencyProperty.Register(
            "FilterOpacity", typeof(double), typeof(N_GrayScale));

        public double FilterOpacity
        {
            get => (double)ViewModel.GetValue(FilterOpacityProperty);
            set => ViewModel.SetValue(FilterOpacityProperty, value);
        }


        public N_GrayScale()
        {
            ViewModel.Title = "Grayscale";
            ViewModel.Options =
            [
                new Slider(),
                new Button() { Content="Button" }
            ];
        }
    }
}
