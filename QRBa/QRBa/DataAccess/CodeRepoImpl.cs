using QRBa.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRBa.Domain;
using Newtonsoft.Json;
using QRBa.Util;

namespace QRBa.DataAccess
{
    public partial class DataAccessor : ICodeRepository
    {
        public Code AddCode(Code newCode)
        {
            var result = QueryStoreProcedure("AddCode", new Dictionary<string, object>
                                                          {
                                                              {"accountId", newCode.AccountId},
                                                              {"codeTypeId", (byte)newCode.Type},
                                                              {"codeRectangle", JsonConvert.SerializeObject(newCode.Rectangle)},
                                                              {"backgroundImage", newCode.BackgroundImage},
                                                              {"backgroundContentType", newCode.BackgroundContentType},
                                                              {"payload", JsonHelper.Serialize(newCode.Payload)},
                                                          });
            if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[1].Rows[0]);
                return code;
            }
            return null;
        }

        public Code GetCode(int accountId, int codeId)
        {
            var result = QueryStoreProcedure("GetCode", new Dictionary<string, object>
                                                          {
                                                              {"accountId", accountId},
                                                              {"codeId", codeId},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[0].Rows[0]);
                return code;
            }
            return null;
        }

        public Code UpdateCode(Code newCode)
        {
            var result = QueryStoreProcedure("UpdateCode", new Dictionary<string, object>
                                                          {
                                                              {"accountId", newCode.AccountId},
                                                              {"codeId", newCode.CodeId},
                                                              {"codeTypeId", (byte)newCode.Type},
                                                              {"codeRectangle", JsonConvert.SerializeObject(newCode.Rectangle)},
                                                              {"backgroundImage", newCode.BackgroundImage},
                                                              {"backgroundContentType", newCode.BackgroundContentType},
                                                              {"payload", JsonHelper.Serialize(newCode.Payload)},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[0].Rows[0]);
                return code;
            }
            return null;
        }
    }
}
