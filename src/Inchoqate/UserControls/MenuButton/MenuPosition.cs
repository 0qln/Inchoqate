using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.UserControls.MenuButton
{
    public enum MenuPosition
    {
        Left = -1,
        Top = 8, 
        Right = 1,
        Bottom = -8,

        TopLeft = Top + Left,
        TopRight = Top + Right,
        BottomLeft = Bottom + Left,
        BottomRight = Bottom + Right
    }
}
