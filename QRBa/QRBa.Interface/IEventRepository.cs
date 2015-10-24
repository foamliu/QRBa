using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Interface
{
    public interface IEventRepository
    {
        Event AddEvent(Event newEvent);
    }
}
