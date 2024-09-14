using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Model
{
    interface IGuidHolder
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        Guid Id { get; }
    }
}
