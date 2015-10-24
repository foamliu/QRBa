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
        public static void SaveBackground(int accountId, int codeId, string contentType, byte[] data)
        {
            var fileName = UrlHelper.Code62Encode(accountId, codeId);
            var ext = GetDefaultExtension(contentType);
            fileName = string.Format("background_{0}{1}", fileName, ext);
            SaveFile(fileName, contentType, data);
        }

        public static void SaveCode(int accountId, int codeId, byte[] data)
        {
            var fileName = UrlHelper.Code62Encode(accountId, codeId);
            var ext = GetDefaultExtension(Constants.ContentType);
            fileName = string.Format("code_{0}{1}", fileName, ext);
            SaveFile(fileName, Constants.ContentType, data);
        }

        public static void SaveThumbnail(int accountId, int codeId, byte[] data)
        {
            var fileName = UrlHelper.Code62Encode(accountId, codeId);
            var ext = GetDefaultExtension(Constants.ContentType);
            fileName = string.Format("thumbnail_{0}{1}", fileName, ext);
            SaveFile(fileName, Constants.ContentType, data);
        }

        public static byte[] GetBackground(int accountId, int codeId, string contentType)
        {
            var fileName = UrlHelper.Code62Encode(accountId, codeId);
            var ext = GetDefaultExtension(contentType);
            fileName = string.Format("background_{0}{1}", fileName, ext);
            return GetFile(fileName);
        }

        public static byte[] GetCode(int accountId, int codeId)
        {
            var fileName = UrlHelper.Code62Encode(accountId, codeId);
            var ext = GetDefaultExtension(Constants.ContentType);
            fileName = string.Format("code_{0}{1}", fileName, ext);
            return GetFile(fileName);
        }

        public static byte[] GetThumbnail(int accountId, int codeId)
        {
            var fileName = UrlHelper.Code62Encode(accountId, codeId);
            var ext = GetDefaultExtension(Constants.ContentType);
            fileName = string.Format("thumbnail_{0}{1}", fileName, ext);
            return GetFile(fileName);
        }

        private static void SaveFile(string fileName, string contentType, byte[] data)
        {
            if (contentType != null && data != null)
            {
                string targetFolder = HttpContext.Current.Server.MapPath(Constants.PictureFolder);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                string targetPath = Path.Combine(targetFolder, fileName);
                File.WriteAllBytes(targetPath, data);
            }
        }

        public static byte[] GetFile(string fileName)
        {
            string targetFolder = HttpContext.Current.Server.MapPath(Constants.PictureFolder);
            string targetPath = Path.Combine(targetFolder, fileName);
            if (File.Exists(targetPath))
            {
                return File.ReadAllBytes(targetPath);
            }

            return null;
        }

        public static string GetDefaultExtension(string mimeType)
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
