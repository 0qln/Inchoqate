using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Main.Editor.FlowChart
{
    public class N_NoRedChannel : NodeModel
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
            "FilterOpacity", typeof(double), typeof(N_NoRedChannel));

        public double FilterOpacity
        {
            get => (double)GetValue(FilterOpacityProperty);
            set => SetValue(FilterOpacityProperty, value);
        }


        public N_NoRedChannel()
        {
            base.Title = "Remove Red Channel";
            base.Options =
            [
                new Slider() { Name="Opacity" }
            ];
        }
    }
}
