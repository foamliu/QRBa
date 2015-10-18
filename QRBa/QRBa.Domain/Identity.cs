using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Domain
{
    [DataContract(Namespace = "")]
    public class Identity
    {
        [DataMember]
        public string MemberName { get; set; }

        [DataMember]
        public string PasswordHash { get; set; }
    }
}
