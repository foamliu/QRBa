using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using ZXing;

namespace QRBa.Util
{
    public static class QrCodeHelper
    {
        public static Bitmap Draw(string barcodeText, Bitmap background, Rectangle destRect)
        {
            int x = destRect.X;
            int y = destRect.Y;
            int width = destRect.Width;
            int height = destRect.Height;

            if (x + width >= background.Width || y + height >= background.Height)
            {
                throw new QRBaException(ErrorConstants.Out_Of_Range);
            }

            var encoder = new QrEncoder(ErrorCorrectionLevel.M);
            var code = encoder.Encode(barcodeText);

            if (width < code.Matrix.Width * 2 || height < code.Matrix.Height * 2)
            {
                throw new QRBaException(ErrorConstants.Window_Too_Small);
            }

            byte[,] map = Calculate(code, background.Width, background.Height, new Rectangle(x, y, width, height));

            for (int i = 0; i < background.Width; i++)
            {
                for (int j = 0; j < background.Height; j++)
                {
                    var c = background.GetPixel(i, j);

                    if (map[i, j] == (byte)ColorType.Dark)
                    {
                        background.SetPixel(i, j, Darker(c));
                    }
                    else if (map[i, j] == (byte)ColorType.Light)
                    {
                        background.SetPixel(i, j, Lighter(c));
                    }

                }
            }

            return background;
        }

        public static void CreateCode(Code code)
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(code.BackgroundImage))
            {
                bmp = new Bitmap(ms);
                if (bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Bitmap newBmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
                    using (Graphics gfx = Graphics.FromImage(newBmp))
                    {
                        gfx.DrawImage(bmp, 0, 0);
                    }
                    bmp = newBmp;
                }
            }
            string url = UrlHelper.GetUrl(code.AccountId, code.CodeId);
            string fileName = UrlHelper.Code62Encode(code.AccountId, code.CodeId);
            fileName = Path.Combine(fileName, ".png");

            Bitmap image = QrCodeHelper.Draw(url, bmp, code.Rectangle);
            byte[] data = QrCodeHelper.ImageToByteArray(image);

            FileHelper.SaveCode(code.AccountId, code.CodeId, data);
        }

        private static bool ThumbnailCallback()
        {
            return true;
        }

        /// <summary>
        /// A thumbnail image is a small version of an image.
        /// </summary>
        /// <param name="code"></param>
        public static void CreateThumbnail(Code code)
        {
            var codeBytes = FileHelper.GetCode(code.AccountId, code.CodeId);
            Image codeImage = ByteArrayToImage(codeBytes);
            Image thumbnailImage = codeImage.GetThumbnailImage(100, 100, new Image.GetThumbnailImageAbort(ThumbnailCallback) , new IntPtr());
            FileHelper.SaveThumbnail(code.AccountId, code.CodeId, ImageToByteArray(thumbnailImage));
        }

        //public static int GetSize(string barcodeText)
        //{
        //    var encoder = new QrEncoder(ErrorCorrectionLevel.M);
        //    var code = encoder.Encode(barcodeText);
        //    return code.Matrix.Width;
        //}

        public static string Decode(byte[] data)
        {
            // create a barcode reader instance
            IBarcodeReader reader = new BarcodeReader();
            // load a bitmap
            var barcodeBitmap = (Bitmap)Bitmap.FromStream(new MemoryStream(data));
            // detect and decode the barcode inside the bitmap
            var result = reader.Decode(barcodeBitmap);
            // do something with the result
            if (result != null)
            {
                //result.BarcodeFormat.ToString()
                //result.ResultPoints;
                return result.Text;
            }
            return null;
        }

        #region Private Methods

        private static Color Darker(Color c)
        {
            const byte delta = 150;
            int r = c.R - delta < 0 ? 0 : c.R - delta;
            int g = c.G - delta < 0 ? 0 : c.G - delta;
            int b = c.B - delta < 0 ? 0 : c.B - delta;

            return Color.FromArgb(r, g, b);
        }

        private static Color Lighter(Color c)
        {
            const byte delta = 150;
            int r = c.R + delta > 255 ? 255 : c.R + delta;
            int g = c.G + delta > 255 ? 255 : c.G + delta;
            int b = c.B + delta > 255 ? 255 : c.B + delta;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 计算矩阵
        /// </summary>
        /// <param name="code"></param>
        /// <param name="width">画布的尺寸</param>
        /// <param name="height">画布的尺寸</param>
        /// <param name="rectangle">画二维码的窗口</param>
        /// <returns></returns>
        private static byte[,] Calculate(QrCode code, int width, int height, Rectangle rectangle)
        {
            const int margin = 5;
            var map = new byte[width, height];

            int matrixW = code.Matrix.Width;
            int matrixH = code.Matrix.Height;

            int n = rectangle.Height > rectangle.Width ? (rectangle.Width - margin * 2) / matrixW : (rectangle.Height - margin * 2) / matrixH;

            int w = matrixW * n;
            int h = matrixH * n;

            int x = rectangle.X + (rectangle.Width - w) / 2 - n;
            int y = rectangle.Y + (rectangle.Height - h) / 2 - n;

            for (int xx = 0; xx < code.Matrix.Width; xx++)
            {
                for (int yy = 0; yy < code.Matrix.Height; yy++)
                {
                    if (code.Matrix.InternalArray[xx, yy])
                    {
                        int xxx = x + n + (n - 1) / 2 + xx * n;
                        int yyy = y + n + (n - 1) / 2 + yy * n;
                        //g.FillRectangle(black, new Rectangle(xxx, yyy, 2, 2));
                        FillRoundedRectangle(map, (byte)ColorType.Dark, new Rectangle(xxx, yyy, 2, 2));
                    }
                    else
                    {
                        int xxx = x + n + (n - 1) / 2 + xx * n;
                        int yyy = y + n + (n - 1) / 2 + yy * n;
                        //g.FillRectangle(white, new Rectangle(xxx, yyy, 2, 2));
                        FillRoundedRectangle(map, (byte)ColorType.Light, new Rectangle(xxx, yyy, 2, 2));
                    }
                }
            }

            DrawFinder(map, x, y, n);
            DrawFinder(map, x + (code.Matrix.Width - 7) * n, y, n);
            DrawFinder(map, x, y + (code.Matrix.Height - 7) * n, n);
            DrawAlignment(map, x + (code.Matrix.Width - 7 - 1) * n, y + (code.Matrix.Height - 7 - 1) * n, n);

            return map;
        }

        private static void FillRoundedRectangle(byte[,] map, byte color, Rectangle rectangle)
        {
            for (int x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    if (CanRoundCorner(rectangle.Width))
                    {
                        if (CheckPoint(map, color, x, y, rectangle.X, rectangle.Y, +1, +1))
                        { }
                        else if (CheckPoint(map, color, x, y, rectangle.X + rectangle.Width - 1, rectangle.Y, -1, +1))
                        { }
                        else if (CheckPoint(map, color, x, y, rectangle.X, rectangle.Y + rectangle.Height - 1, +1, -1))
                        { }
                        else if (CheckPoint(map, color, x, y, rectangle.X + rectangle.Width - 1, rectangle.Y + rectangle.Height - 1, -1, -1))
                        { }
                        else
                        {
                            map[x, y] = color;
                        }
                    }
                    else
                    {
                        map[x, y] = color;
                    }
                }
            }
        }

        private static void FillRectangle(byte[,] map, byte color, Rectangle rectangle)
        {
            for (int x = rectangle.X; x < rectangle.X + rectangle.Width; x++)
            {
                for (int y = rectangle.Y; y < rectangle.Y + rectangle.Height; y++)
                {
                    map[x, y] = color;
                }
            }
        }

        private static bool CheckPoint(byte[,] map, byte color, int x, int y, int m, int n, int signX, int signY)
        {
            const int radius = 2;

            bool result = Math.Abs(x - m) <= radius && Math.Abs(y - n) <= radius;

            int xx = m + radius * signX;
            int yy = n + radius * signY;

            if ((x - xx) * (x - xx) + (y - yy) * (y - yy) <= radius * radius)
            {
                map[x, y] = color;
            }

            return result;
        }

        private static bool CanRoundCorner(int size)
        {
            const int radius = 2;

            return size > radius * 2;
        }

        private static void DrawFinder(byte[,] map, int x, int y, int N)
        {
            FillRoundedRectangle(map, (byte)ColorType.Light, new Rectangle(x + 0 * N, y + 0 * N, 9 * N, 9 * N));
            FillRoundedRectangle(map, (byte)ColorType.Dark, new Rectangle(x + 1 * N, y + 1 * N, 7 * N, 7 * N));
            FillRoundedRectangle(map, (byte)ColorType.Light, new Rectangle(x + 2 * N, y + 2 * N, 5 * N, 5 * N));
            FillRoundedRectangle(map, (byte)ColorType.Dark, new Rectangle(x + 3 * N, y + 3 * N, 3 * N, 3 * N));
        }

        private static void DrawAlignment(byte[,] map, int x, int y, int N)
        {
            FillRoundedRectangle(map, (byte)ColorType.Dark, new Rectangle(x + 0 * N, y + 0 * N, 5 * N, 5 * N));
            FillRoundedRectangle(map, (byte)ColorType.Light, new Rectangle(x + 1 * N, y + 1 * N, 3 * N, 3 * N));
            FillRectangle(map, (byte)ColorType.Dark, new Rectangle(x + 2 * N, y + 2 * N, 1 * N, 1 * N));
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            ImageCodecInfo jpgEncoder;
            EncoderParameters encoderParams;

            GetJpegParams(Constants.JpegCompressionLevel, out jpgEncoder, out encoderParams);
            imageIn.Save(ms, jpgEncoder, encoderParams);
            return ms.ToArray();
        }


        public static void GetJpegParams(long compressionLevel, out ImageCodecInfo jpgEncoder, out EncoderParameters myEncoderParameters)
        {
            jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            Encoder myEncoder = Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, compressionLevel);
            myEncoderParameters.Param[0] = myEncoderParameter;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        #endregion
    }
}