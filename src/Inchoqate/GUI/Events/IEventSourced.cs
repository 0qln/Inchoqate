using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inchoqate.GUI.Events
{
    public interface IEventSourced
    {
        public Guid Id { get; init; }

        public void Apply(Event @event);
    }
}
