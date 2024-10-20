using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Model.Events
{
    public class DummyEvent : EventModel, IDeserializable<DummyEvent>
    {
        /// <inheritdoc />
        protected override bool InnerDo()
        {
            return true;
        }

        /// <inheritdoc />
        protected override bool InnerUndo()
        {
            return true;
        }
    }
}
