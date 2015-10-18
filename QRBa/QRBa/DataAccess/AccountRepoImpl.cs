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
            throw new NotImplementedException();
        }

        public void AddAccoutIdentityMapping(long accountId, byte identityType, string identityValue)
        {
            throw new NotImplementedException();
        }

        public Identity AddIdentity(Identity newIdentity)
        {
            throw new NotImplementedException();
        }

        public bool CheckIdentity(Identity identity)
        {
            throw new NotImplementedException();
        }

        public Account GetAccountByIdentity(IdentityType idType, string idValue)
        {
            throw new NotImplementedException();
        }

        public Account UpdateAccount(Account newAccount)
        {
            throw new NotImplementedException();
        }
    }
}
