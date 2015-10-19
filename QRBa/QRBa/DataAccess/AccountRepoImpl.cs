using QRBa.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRBa.Domain;

namespace QRBa.DataAccess
{
    public partial class DataAccessor : IAccountRepository
    {
        public Account AddAccount(Account newAccount)
        {
            long newAccountId = accountIdGen.NextId;
            var result = QueryStoreProcedure("AddAccount", new Dictionary<string, object>
                                                          {
                                                              {"id", newAccountId},
                                                              {"name", newAccount.Name??string.Empty},
                                                              {"email", newAccount.Email??string.Empty},
                                                              {"statusId", (byte)AccountStatusType.Active},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var acct = new Account().FromRow(result.Tables[0].Rows[0]);
                return acct;
            }
            return null;
        }

        public void AddAccoutIdentity(int accountId, byte identityType, string identityValue)
        {
            var result = QueryStoreProcedure("AddAccountIdentity", new Dictionary<string, object>
                                                          {
                                                              {"accountId", accountId},
                                                              {"identityTypeId", identityType},
                                                              {"identityValue", identityValue},
                                                          });
        }

        public Identity AddIdentity(Identity newIdentity)
        {
            var result = QueryStoreProcedure("AddIdentity", new Dictionary<string, object>
                                                          {
                                                              {"memberName", newIdentity.MemberName},
                                                              {"passwordHash", newIdentity.PasswordHash}
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                return new Identity().FromRow(result.Tables[0].Rows[0]);
            }
            return null;
        }

        public bool CheckIdentity(Identity identity)
        {
            var result = QueryStoreProcedure("GetIdentity", new Dictionary<string, object>
                                                          {
                                                              {"memberName", identity.MemberName}
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var idInDb = new Identity().FromRow(result.Tables[0].Rows[0]);
                return idInDb.MemberName == identity.MemberName && idInDb.PasswordHash == identity.PasswordHash;
            }
            return false;
        }

        public Account GetAccountByIdentity(IdentityType idType, string idValue)
        {
            var result = QueryStoreProcedure("GetAccountByIdentity", new Dictionary<string, object>
                                                          {
                                                              {"identityTypeId", (byte)idType},
                                                              {"identityValue", idValue},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var acct = new Account().FromRow(result.Tables[0].Rows[0]);
                return acct;
            }
            return null;
        }

        public Account UpdateAccount(Account newAccount)
        {
            var result = QueryStoreProcedure("UpdateAccount", new Dictionary<string, object>
                                                          {
                                                              {"id", newAccount.Id},
                                                              {"name", newAccount.Name},
                                                              {"email", newAccount.Email},
                                                              {"statusId", (byte)newAccount.Status},
                                                          });
            if (result.Tables[0].Rows.Count > 0)
            {
                var acct = new Account().FromRow(result.Tables[0].Rows[0]);
                return acct;
            }
            return null;
        }
    }
}
