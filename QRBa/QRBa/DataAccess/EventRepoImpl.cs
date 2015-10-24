using QRBa.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRBa.Domain;

namespace QRBa.DataAccess
{
    public partial class DataAccessor : IEventRepository
    {
        public Event AddEvent(Event newEvent)
        {
            var result = QueryStoreProcedure("AddEvent", new Dictionary<string, object>
                                                          {
                                                              {"param_accountId", newEvent.AccountId},
                                                              {"param_codeId", newEvent.CodeId},
                                                              {"param_eventTypeId", (byte)newEvent.Type},
                                                              {"param_payload", newEvent.Payload},
                                                          });
            if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
            {
                var evt = new Event().FromRow(result.Tables[1].Rows[0]);
                return evt;
            }
            return null;
        }
    }
}
