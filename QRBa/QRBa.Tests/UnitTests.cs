using Microsoft.VisualStudio.TestTools.UnitTesting;
using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thought.vCards;
using ZXing;

namespace QRBa.Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void UT_QrCodeTest()
        {
            Random rand = new Random();
            int adId = rand.Next();
            long accountId = rand.Next();
            var url = UrlHelper.GetUrl(adId, accountId);
            byte[] data;
            const int count = 5;

            for (int i = 0; i < count; i++)
            {
                foreach (var file in new DirectoryInfo("Images").GetFiles("image_*.jpg"))
                {
                    using (Bitmap bmp = (Bitmap)Bitmap.FromFile(file.FullName))
                    {
                        int x, y, width, height;
                        do
                        {
                            x = rand.Next() % (bmp.Width * 3 / 4);
                            y = rand.Next() % (bmp.Height * 3 / 4);
                            width = rand.Next() % bmp.Width / 4;
                            //height = rand.Next() % bmp.Height / 4;
                            height = width;
                        }
                        while (x + width >= bmp.Width || y + height >= bmp.Height || width < 80 || width > 400);

                        var temp = QrCodeHelper.Draw(url, bmp, new Rectangle(x, y, width, height));

                        Bitmap clip = new Bitmap(width, height);
                        using (Graphics g = Graphics.FromImage(clip))
                        {
                            g.DrawImage(temp, new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                        }
                        data = QrCodeHelper.ImageToByteArray(clip);

                        File.WriteAllBytes("temp.png", data);
                        string barcodeText = QrCodeHelper.Decode(data);
                        Assert.AreEqual(url, barcodeText);
                    }
                }
            }
        }

        [TestMethod]
        public void UT_DecodeTest()
        {
            // create a barcode reader instance
            IBarcodeReader reader = new BarcodeReader();
            // load a bitmap
            Bitmap barcodeBitmap = null;
            barcodeBitmap = (Bitmap)Bitmap.FromFile(@"C:\temp\heart.jpg");

            if (barcodeBitmap != null)
            {
                // detect and decode the barcode inside the bitmap
                var result = reader.Decode(barcodeBitmap);
                // do something with the result
                if (result != null)
                {
                    Console.WriteLine(result.BarcodeFormat.ToString());
                    Console.WriteLine(result.Text);
                }
            }
        }

        [TestMethod]
        public void UT_EncodeTest()
        {
            string url = "http://teal.azurewebsites.net/Event/1/1";
            var encoder = new Gma.QrCodeNet.Encoding.QrEncoder(Gma.QrCodeNet.Encoding.ErrorCorrectionLevel.M);
            var code = encoder.Encode(url);
            var bmp = new Bitmap(code.Matrix.Width, code.Matrix.Height);
            for (int x = 0; x < code.Matrix.Width; x++)
            {
                for (int y = 0; y < code.Matrix.Height; y++)
                {
                    if (code.Matrix.InternalArray[x, y])
                    {
                        bmp.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        bmp.SetPixel(x, y, Color.White);
                    }
                }
            }
            bmp.Save("bmp.png", ImageFormat.Png);
        }

        [TestMethod]
        public void Code62Test()
        {
            Assert.AreEqual("0", UrlHelper.Code62Encode(0));
            Assert.AreEqual("1", UrlHelper.Code62Encode(1));
            Assert.AreEqual("10", UrlHelper.Code62Encode(62));
            Assert.AreEqual("2LKcb1", UrlHelper.Code62Encode(int.MaxValue));
            Assert.AreEqual(int.MaxValue, UrlHelper.Code62Decode("2LKcb1"));
        }

        [TestMethod]
        public void vCardLibTest()
        {
            vCard card = new vCard();

            // Simple properties

            //card.AdditionalNames = AdditionalNames.Text;
            card.FamilyName = "刘";
            card.FormattedName = "刘杨";
            card.GivenName = "杨";
            //card.NamePrefix = NamePrefix.Text;
            //card.NameSuffix = NameSuffix.Text;
            card.Organization = "微软";
            //card.Role = Role.Text;
            card.Title = "高级研发经理";

            // ---------------------------------------------------------------
            // Email Addresses
            // ---------------------------------------------------------------
            // A vCard supports any number of email addresses.


            //card.EmailAddresses.Add(
            //    new vCardEmailAddress("浦驰路 1809弄 104号 701"));


            // ---------------------------------------------------------------
            // Nicknames
            // ---------------------------------------------------------------

            //string[] nicklist = new string[] { "泡沫刘" };
            //foreach (string nick in nicklist)
            //{
            //    if (nick.Length > 0)
            //        card.Nicknames.Add(nick);
            //}


            // ---------------------------------------------------------------
            // Notes
            // ---------------------------------------------------------------
            // The vCard specification allows for multiple notes, although
            // most applications seem to support a maximum of one note.

            //if (Note.Text.Length > 0)
            //{
            //    card.Notes.Add(new vCardNote(Note.Text));
            //}


            // ---------------------------------------------------------------
            // Phones
            // ---------------------------------------------------------------
            //
            // A vCard supports any number of telephones.  Each telephone can
            // have a different type (such as a video phone or a fax) and a
            // purpose (e.g. a home number or a work number).

            //if (!string.IsNullOrEmpty(WorkPhone.Text))
            //{
            //    card.Phones.Add(
            //        new vCardPhone(WorkPhone.Text, vCardPhoneTypes.WorkVoice));
            //}

            card.Phones.Add(
                new vCardPhone("13681765654", vCardPhoneTypes.Cellular));


            //if (!string.IsNullOrEmpty(WorkFax.Text))
            //{
            //    card.Phones.Add(
            //        new vCardPhone(WorkFax.Text, vCardPhoneTypes.WorkFax));
            //}


            // ---------------------------------------------------------------
            // Web Sites
            // ---------------------------------------------------------------

            //if (WorkWebSite.Text.Length > 0)
            //{
            //    card.Websites.Add(
            //        new vCardWebsite(WorkWebSite.Text, vCardWebsiteTypes.Work));
            //}

            var writer = new vCardStandardWriter();
            writer.EmbedInternetImages = false;
            writer.EmbedLocalImages = true;
            writer.Options = vCardStandardWriterOptions.IgnoreCommas;

            StringBuilder sb = new StringBuilder();
            using (var textWriter = new StringWriter(sb))
            {
                writer.Write(card, textWriter);
            }

            var expected = @"BEGIN:VCARD
N:刘;杨;;;
FN:刘杨
ORG:微软
TEL;CELL:13681765654
TITLE:高级研发经理
END:VCARD
";
            Assert.AreEqual(expected, sb.ToString());
        }
    }
}
