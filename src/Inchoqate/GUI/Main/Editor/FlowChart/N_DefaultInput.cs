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
    internal class N_DefaultInput : Node, INode
    {
        public bool RequiresBreak => false;

        private List<INode> _outputs = [];

        public List<INode>? Next
        {
            get
            {
                return _outputs;
            }
        }

        public List<INode>? Prev
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
