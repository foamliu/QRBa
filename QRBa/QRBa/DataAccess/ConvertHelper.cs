using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.DataAccess
{
    public static class ConvertHelper
    {
        public static Identity FromRow(this Identity ti, DataRow row)
        {
            ti.MemberName = row.GetStringField("MemberName");
            ti.PasswordHash = row.GetStringField("PasswordHash");
            return ti;
        }

        public static Account FromRow(this Account acct, DataRow row)
        {
            acct.Id = row.GetIntField("Id");
            acct.Name = row.GetStringField("Name");
            acct.Email = row.GetStringField("Email");
            acct.Status = (AccountStatusType)(row.GetByteField("StatusId"));
            return acct;
        }
    }
}
