using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI
{
    public interface IDraggable
    {
    }

    public static class DragExtensions
    {
        public static void RegisterDrag(this IDraggable @object, Control handle)
        {
        }
    }
}
