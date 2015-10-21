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
                                                              {"param_accountId", newCode.AccountId},
                                                              {"param_codeTypeId", (byte)newCode.Type},
                                                              {"param_codeRectangle", JsonConvert.SerializeObject(newCode.Rectangle)},
                                                              {"param_backgroundContentType", newCode.BackgroundContentType},
                                                              {"param_payload", JsonHelper.Serialize(newCode.Payload)},
                                                          });
            if (result.Tables.Count > 1 && result.Tables[1].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[1].Rows[0]);
                code.BackgroundImage = newCode.BackgroundImage;
                FileHelper.SaveBackground(code.AccountId, code.CodeId, code.BackgroundContentType, code.BackgroundImage);
                return code;
            }
            return null;
        }

        public Code GetCode(int accountId, int codeId)
        {
            var result = QueryStoreProcedure("GetCode", new Dictionary<string, object>
                                                          {
                                                              {"param_accountId", accountId},
                                                              {"param_codeId", codeId},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[0].Rows[0]);
                code.BackgroundImage = FileHelper.GetBackground(code.AccountId, code.CodeId, code.BackgroundContentType);
                return code;
            }
            return null;
        }

        public List<Code> GetCodes(int accountId)
        {
            var list = new List<Code>();
            var result = QueryStoreProcedure("GetCodes", new Dictionary<string, object>
                                                          {
                                                              {"param_accountId", accountId},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[0].Rows[0]);
                code.BackgroundImage = FileHelper.GetBackground(code.AccountId, code.CodeId, code.BackgroundContentType);
                code.Thumbnail = FileHelper.GetThumbnail(code.AccountId, code.CodeId);
                list.Add(code);
            }
            return list;
        }

        public Code UpdateCode(Code newCode)
        {
            var result = QueryStoreProcedure("UpdateCode", new Dictionary<string, object>
                                                          {
                                                              {"param_accountId", newCode.AccountId},
                                                              {"param_codeId", newCode.CodeId},
                                                              {"param_codeTypeId", (byte)newCode.Type},
                                                              {"param_codeRectangle", JsonConvert.SerializeObject(newCode.Rectangle)},
                                                              {"param_backgroundContentType", newCode.BackgroundContentType},
                                                              {"param_payload", JsonHelper.Serialize(newCode.Payload)},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var code = new Code().FromRow(result.Tables[0].Rows[0]);

                if (newCode.BackgroundImage != null)
                {
                    FileHelper.SaveBackground(code.AccountId, code.CodeId, code.BackgroundContentType, newCode.BackgroundImage);
                    code.BackgroundImage = newCode.BackgroundImage;
                }
                else
                {
                    code.BackgroundImage = FileHelper.GetBackground(code.AccountId, code.CodeId, code.BackgroundContentType);
                }

                return code;
            }
            return null;
        }
    }
}
