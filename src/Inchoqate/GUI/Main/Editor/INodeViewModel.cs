using Inchoqate.GUI.Main.Editor.FlowChart;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Main.Editor
{
    public class NodeCollection<T> : ObservableCollection<T>
        where T : INodeViewModel
    {
    }

    public interface INodeViewModel
    {
        public NodeCollection Outputs { get; set; }
        public NodeCollection Inputs { get; set; }

        public void AddNext(INodeViewModel next);
    }
}
