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
            return request.Cookies[name].Value;
        }
    }
}
