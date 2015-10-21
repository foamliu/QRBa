using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QRBa.Util
{
    public static class Constants
    {
        public const long DummyAccountId = -1;

        public const string AccountId = "AccountId";
        public const string ContentType = "image/png";
        public const string PictureFolder = "~/Pictures";

        public const string BaseUrl = "http://qrba.cc/";
    }

    public static class ErrorConstants
    {
        public const string Out_Of_Range = "二维码没法完全放入背景图片";
        public const string Window_Too_Small = "二维码窗口太小";
    }
}
