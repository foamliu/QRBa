using QRBa.Domain;
using QRBa.Util;
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

        public static string GetImageString(this Code code)
        {
            return code.BackgroundImage != null ? (string.Format("data:{0};base64,{1}", code.BackgroundContentType, Convert.ToBase64String(code.BackgroundImage))) : null;
        }

        public static string GetThumbnailString(this Code code)
        {
            return code.Thumbnail != null ? (string.Format("data:{0};base64,{1}", Constants.ContentType, Convert.ToBase64String(code.Thumbnail))) : null;
        }

        public static string GetImagePath(this Code code)
        {
            var fileName = UrlHelper.Code62Encode(code.AccountId, code.CodeId);
            var ext = FileHelper.GetDefaultExtension(code.BackgroundContentType);
            fileName = string.Format("/Pictures/background_{0}{1}", fileName, ext);
            return fileName;
        }

        public static string GetThumbnailPath(this Code code)
        {
            var fileName = UrlHelper.Code62Encode(code.AccountId, code.CodeId);
            fileName = string.Format("/Pictures/thumbnail_{0}.jpg", fileName);
            return fileName;
        }

    }
}
