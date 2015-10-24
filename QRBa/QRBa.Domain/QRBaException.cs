using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Domain
{
    public class QRBaException : Exception
    {
        public QRBaException(string message) : base(message) { }
    }
}
