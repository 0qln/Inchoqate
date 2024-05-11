using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    internal class N_DefaultOutput : Node, INode
    {
        public bool RequiresBreak => false;

        private INode _input;

        public List<INode>? Next
        {
            get
            {
                return null;
            }
        }

        public List<INode>? Prev
        {
            get
            {
                return [_input];
            }
        }


        public N_DefaultOutput()
        {
            base.Title = "Result";
            base.Options =
            [
            ];
        }
    }
}
