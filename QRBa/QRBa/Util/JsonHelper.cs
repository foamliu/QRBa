using Newtonsoft.Json;
using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Util
{
    public static class JsonHelper
    {
        public static string Serialize(Payload payload)
        {
            if (payload is UrlPayload)
            {
                var p = (UrlPayload)payload;
                return JsonConvert.SerializeObject(p);
            }
            return string.Empty;
        }

        public static Payload Deserialize(string json, CodeType type)
        {
            if (json != null)
            {
                switch (type)
                {
                    case CodeType.Url:
                        return JsonConvert.DeserializeObject<UrlPayload>(json);
                    case CodeType.vCard:
                        return JsonConvert.DeserializeObject<vCardPayload>(json);
                    default:
                        throw new NotSupportedException();
                }
            }
            return null;
        }

    }
}
