using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Inchoqate.GUI.Main.Editor.Panel
{
    /// <summary>
    /// The Panel Editor is a linear, squential pipeline of simple edits.
    /// 
    ///                    Side Panel
    /// +-----------------+---------+
    /// |                 | =edit6= | Pipeline Index 1 (collapsed)
    /// |                 |         |
    /// |  Preview Image  | =edit3= | Pipeline Index 2 (visible)
    /// |                 |  opt1   |
    /// |                 |  opt2   |
    /// |                 |         |
    /// +-----------------+---------+
    /// 
    /// </summary>
    public partial class PanelEditor : UserControl
    {
        public PanelEditor()
        {
            InitializeComponent();
        }
    }
}
