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
            int accountId = GetAccountId();

            var code = DataAccessor.CodeRepository.AddCode(
                new Code
                {
                    AccountId = accountId,
                    Type = CodeType.Url,
                    Payload = new UrlPayload { TargetingUrl = url }
                });

            return View("Select", code);
        }

        private int GetAccountId()
        {
            var cookie = Request.Cookies[Constants.AccountId];

            if (cookie == null)
            {
                var account = DataAccessor.AccountRepository.AddAccount(new Account { });
                CookieHelper.SetCookie(Response, Constants.AccountId, account.Id.ToString(), true);
                return account.Id;
            }
            else
            {
                return Convert.ToInt32(cookie.Value);
            }
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
            int x = (int)Math.Round(Convert.ToDouble(Request["x"].ToString()));
            int y = (int)Math.Round(Convert.ToDouble(Request["y"].ToString()));
            int width = (int)Math.Round(Convert.ToDouble(Request["width"].ToString()));
            int height = (int)Math.Round(Convert.ToDouble(Request["height"].ToString()));

            code.Rectangle = new Rectangle(x, y, width, height);
            code = DataAccessor.CodeRepository.UpdateCode(code);

            QrCodeHelper.CreateCode(code);
            QrCodeHelper.CreateThumbnail(code);

            return RedirectToAction("Dashboard", "Generator");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Dashboard()
        {
            int accountId = GetAccountId();
            var codes = DataAccessor.CodeRepository.GetCodes(accountId);
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult Download()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public FileResult Download(int codeId)
        {
            var accountId = Convert.ToInt32(CookieHelper.GetCookie(Request, Constants.AccountId));
            //var code = DataAccessor.CodeRepository.GetCode(accountId, codeId);
            var data = FileHelper.GetCode(accountId, codeId);
            return File(data, Constants.ContentType);
        }
    }
}