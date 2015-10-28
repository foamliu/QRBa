using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QRBa.Util
{
    public static class CookieHelper
    {
        public static void SetCookie(HttpResponseBase response, string name, string value, bool rememberMe)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.Value = value;

            if (rememberMe)
            {
                cookie.Expires.AddYears(1);
            }

            response.Cookies.Add(cookie);
        }

        public static string GetCookie(HttpRequestBase request, string name)
        {
            var cookie = request.Cookies[name];
            return cookie == null? null : cookie.Value;
        }

        public static void AddAnonymousCodeId(HttpRequestBase request, HttpResponseBase response, int codeId)
        {
            var cookie = request.Cookies[Constants.AnonymousCodeIdList];
            List<int> codeList = null;
            if (cookie != null)
            {
                codeList = JsonConvert.DeserializeObject<List<int>>(cookie.Value);
            }
            else
            {
                codeList = new List<int>();
            }

            if (!codeList.Contains(codeId))
            {
                codeList.Add(codeId);
            }

            cookie = new HttpCookie(Constants.AnonymousCodeIdList);
            cookie.Value = JsonConvert.SerializeObject(codeList);
            cookie.Expires.AddYears(1);
            response.Cookies.Add(cookie);
        }

        public static List<int> GetAnonymousCodeIdList(HttpRequestBase request)
        {
            var cookie = request.Cookies[Constants.AnonymousCodeIdList];
            List<int> codeList = null;
            if (cookie != null)
            {
                codeList = JsonConvert.DeserializeObject<List<int>>(cookie.Value);
            }
            else
            {
                codeList = new List<int>();
            }
            return codeList;
        }

        public static void ClearAnonymousCodeIdList(HttpResponseBase response)
        {
            HttpCookie c = new HttpCookie(Constants.AnonymousCodeIdList);
            c.Expires = DateTime.Now.AddDays(-1);
            response.Cookies.Add(c);
        }
    }
}
