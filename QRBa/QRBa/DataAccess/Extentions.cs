using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.DataAccess
{
    public static class Extentions
    {
        public static string GetStringField(this DataRow row, string columnName)
        {
            return row[columnName] == DBNull.Value ? null : Convert.ToString(row[columnName]);
        }

        public static Byte GetByteField(this DataRow row, string columnName)
        {
            return Convert.ToByte(row[columnName]);
        }

        public static int GetIntField(this DataRow row, string columnName)
        {
            return Convert.ToInt32(row[columnName]);
        }

        public static long GetInt64Field(this DataRow row, string columnName)
        {
            return Convert.ToInt64(row[columnName]);
        }

        public static decimal GetDecimalField(this DataRow row, string columnName)
        {
            return Convert.ToDecimal(row[columnName]);
        }

        public static byte[] GetByteArray(this DataRow row, string columnName)
        {
            return row[columnName] != DBNull.Value ? (byte[])row[columnName] : null;
        }

        public static DateTime GetDateTimeField(this DataRow row, string columnName)
        {
            return Convert.ToDateTime(row[columnName]);
        }

        public static Guid GetGuidField(this DataRow row, string columnName)
        {
            return new Guid(Convert.ToString(row[columnName]));
        }
    }
}
