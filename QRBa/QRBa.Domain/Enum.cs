using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Domain
{
    [DataContract]
    public enum IdentityType : byte
    {
        [EnumMember]
        QRBaId = 0
    }

    [DataContract]
    public enum AccountStatusType : byte
    {
        [EnumMember]
        Active = 0,

        [EnumMember]
        Locked = 1,

        [EnumMember]
        Closed = 2
    }
}
