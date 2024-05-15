using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    internal class N_DefaultOutput : NodeModel
    {
        public override bool RequiresBreak => false;


        public override List<NodeModel>? Next
        {
            get
            {
                return null;
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


        public N_DefaultOutput()
        {
            base.Title = "Result";
            base.Options =
            [
            ];
        }


        public override void SetNext(NodeModel next)
        {
            throw new InvalidOperationException();
        }
    }
}
