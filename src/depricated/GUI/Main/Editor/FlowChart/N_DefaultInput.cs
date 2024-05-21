using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    internal class N_DefaultInput : NodeModel
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

        
        public override List<NodeModel>? Prev
        {
            get
            {
                return null;
            }
        }


        public N_DefaultInput()
        {
            base.Title = "Image";
            base.Options =
            [
            ];
        }
    }
}
