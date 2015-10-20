using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QRBa.Util
{
    public static class FileHelper
    {
        public static void AddFile(int accountId, int codeId, string contentType, byte[] data)
        {
            if (contentType != null && data != null)
            {
                string targetFolder = HttpContext.Current.Server.MapPath("~/Uploads");
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                string fileName = UrlHelper.Code62Encode(accountId, codeId);
                fileName += GetDefaultExtension(contentType);

                string targetPath = Path.Combine(targetFolder, fileName);
                File.WriteAllBytes(targetPath, data);
            }
        }

        public static byte[] GetFile(int accountId, int codeId, string contentType)
        {
            if (contentType != null)
            {
                string fileName = UrlHelper.Code62Encode(accountId, codeId);
                fileName += "." + GetDefaultExtension(contentType);
                string targetFolder = HttpContext.Current.Server.MapPath("~/Uploads");
                string targetPath = Path.Combine(targetFolder, fileName);
                if (File.Exists(targetPath))
                {
                    return File.ReadAllBytes(targetPath);
                }
            }
            return null;
        }

        private static string GetDefaultExtension(string mimeType)
        {
            string result;
            RegistryKey key;
            object value;

            key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + mimeType, false);
            value = key != null ? key.GetValue("Extension", null) : null;
            result = value != null ? value.ToString() : string.Empty;

            return result;
        }
    }
}
