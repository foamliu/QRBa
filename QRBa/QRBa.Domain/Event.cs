using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Domain
{
    public class Event
    {
        public int AccountId { get; set; }

        public int CodeId { get; set; }

        public EventType Type { get; set; }

        public string Payload { get; set; }
    }
}
