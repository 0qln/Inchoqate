using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Main.Editor.FlowChart
{
    public interface INode
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
        public bool RequiresBreak { get; }

        /// <summary>
        /// Each node produces one output, which can be the input to multiple nodes.
        /// </summary>
        public List<INode> Next { get; }
        
        /// <summary>
        /// The inputs the this node.
        /// </summary>
        public List<INode> Prev { get; }
    }
}
