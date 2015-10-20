using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Domain
{
    public class Code
    {
        public int AccountId { get; set; }

        public int CodeId { get; set; }

        public CodeType Type { get; set; }

        public Rectangle Rectangle { get; set; }

        public byte[] BackgroundImage { get; set; }

        public string BackgroundContentType { get; set; }

        public Payload Payload { get; set; }
    }

    public abstract class Payload
    {

    }

    public class UrlPayload : Payload
    {
        public string TargetingUrl { get; set; }
    }

    public class vCardPayload : Payload
    {

    }
}
