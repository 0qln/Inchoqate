using Inchoqate.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Main.Editor
{
    interface IEditor : IDisposable
    {
        public event EventHandler? Disposing;

        public Texture Source { get; set; }
        public Texture Result { get; }
    }
}
