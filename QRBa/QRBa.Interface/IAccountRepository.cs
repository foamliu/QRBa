using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Interface
{
    public interface IAccountRepository
    {
        Identity AddIdentity(Identity newIdentity);

        void AddAccoutIdentity(int accountId, byte identityType, string identityValue);

        bool CheckIdentity(Identity identity);

        Account AddAccount(Account newAccount);

        Account UpdateAccount(Account newAccount);

        Account GetAccountByIdentity(IdentityType idType, string idValue);
    }
}
