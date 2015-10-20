using QRBa.DataAccess;
using QRBa.Domain;
using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRBa.Controllers
{
    public class GeneratorController : Controller
    {
        // GET: Generator
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create()
        {
            string url = Request["url"].ToString();
            int accountId;

            var cookie = Request.Cookies[Constants.AccountId];

            if (cookie == null)
            {
                var account = DataAccessor.AccountRepository.AddAccount(new Account { });
                CookieHelper.SetCookie(Response, Constants.AccountId, account.Id.ToString(), true);
                accountId = account.Id;
            }
            else
            {
                accountId = Convert.ToInt32(cookie.Value);
            }

            var code = DataAccessor.CodeRepository.AddCode(
                new Code
                {
                    AccountId = accountId,
                    Type = CodeType.Url,
                    Payload = new UrlPayload { TargetingUrl = url }
                });

            return View("Select", code);
        }

        [HttpGet]
        public ActionResult Select()
        {
            return RedirectToAction("Index", "Generator");
        }

        [HttpPost]
        public ActionResult Select(Code code)
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    MemoryStream target = new MemoryStream();
                    file.InputStream.CopyTo(target);
                    code.BackgroundImage = target.ToArray();
                    code.BackgroundContentType = file.ContentType;

                    code = DataAccessor.CodeRepository.UpdateCode(code);

                    return View("Place", code);
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult Place()
        {
            return RedirectToAction("Index", "Generator");
        }

        [HttpPost]
        public ActionResult Place(Code code)
        {
            int x = Convert.ToInt32(Request["x"].ToString());
            int y = Convert.ToInt32(Request["y"].ToString());
            int width = Convert.ToInt32(Request["width"].ToString());
            int height = Convert.ToInt32(Request["height"].ToString());

            code.Rectangle = new Rectangle(x, y, width, height);
            code = DataAccessor.CodeRepository.UpdateCode(code);

            return View("Dashboard");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public FileResult Download(int codeId)
        {
            var accountId = Convert.ToInt32(CookieHelper.GetCookie(Request, Constants.AccountId));
            var code = DataAccessor.CodeRepository.GetCode(accountId, codeId);

            Bitmap bmp;
            using (var ms = new MemoryStream(code.BackgroundImage))
            {
                bmp = new Bitmap(ms);
            }
            string url = Util.UrlHelper.GetUrl(code.AccountId, code.CodeId);

            Bitmap image = QrCodeHelper.Draw(url, bmp, code.Rectangle);
            byte[] data = QrCodeHelper.ImageToByteArray(image);

            return File(data, "image/png", "广告文案.png");
        }
    }
}