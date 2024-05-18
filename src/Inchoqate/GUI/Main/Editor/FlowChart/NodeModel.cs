using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    public abstract class NodeModel : NodeView
    {
        /// <summary>
        /// Wether this node requires a break in the compute chain.
        /// e.g. 
        /// -   A sorter requires all pixels in it's scopre from the prev node 
        ///     to be edited, in order to compare or count them according to the 
        ///     new properties. 
        /// -   Nodes that change a single pixel based on the pixels
        ///     properties alone can be computed in a single shader pass.
        /// -   A better blur than O(n^2) is computed via multipass and thus requires
        ///     a break in the compute chain.
        /// </summary>
        public abstract bool RequiresBreak { get; }

        
        /// <summary>
        /// Each node produces one output, which can be the input to multiple nodes.
        /// </summary>
        public abstract List<NodeModel>? Next { get; }


        /// <summary>
        /// The inputs to this node.
        /// </summary>
        public abstract List<NodeModel>? Prev { get; }


        


        public virtual void SetNext(NodeModel next)
        {
            // Update Model.
            this.Next?.Add(next);
            next.Prev?.Add(this);

            // Update view.
            base.AddNext(next);
        }
    }
}
