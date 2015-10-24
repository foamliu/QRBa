using Newtonsoft.Json;
using QRBa.Domain;
using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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

        public static Code FromRow(this Code code, DataRow row)
        {
            code.AccountId = row.GetIntField("AccountId");
            code.CodeId = row.GetIntField("CodeId");
            code.Type = (CodeType)row.GetByteField("CodeTypeId");
            code.Rectangle = JsonConvert.DeserializeObject<Rectangle>(row.GetStringField("CodeRectangle"));
            code.BackgroundContentType = row.GetStringField("BackgroundContentType");
            code.Payload = JsonHelper.Deserialize(row.GetStringField("Payload"), code.Type);
            return code;
        }

        public static Event FromRow(this Event evt, DataRow row)
        {
            evt.AccountId = row.GetIntField("AccountId");
            evt.CodeId = row.GetIntField("CodeId");
            evt.Type = (EventType)row.GetByteField("EventTypeId");
            evt.Payload = row.GetStringField("Payload");
            return evt;
        }

        public static Comment FromRow(this Comment comment, DataRow row)
        {
            comment.AccountId = row.GetIntField("AccountId");
            comment.CommentId = row.GetIntField("CommentId");
            comment.Content = row.GetStringField("Content");
            comment.InsertedDateTime = row.GetDateTimeField("InsertedDateTime");
            return comment;
        }
    }
}
