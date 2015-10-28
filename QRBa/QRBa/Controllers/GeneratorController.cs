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
using System.Web.Security;

namespace QRBa.Controllers
{
    public class GeneratorController : BaseController
    {
        // GET: Generator
        [HttpGet]
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
                    if (file.ContentLength > 300 * 1024)
                    {
                        Danger("图片太大，无法上传。 ╮(╯▽╰)╭");
                        return View();
                    }

                    var fileName = Path.GetFileName(file.FileName);
                    MemoryStream target = new MemoryStream();
                    file.InputStream.CopyTo(target);
                    code.BackgroundImage = target.ToArray();
                    code.BackgroundContentType = file.ContentType;

                    code = DataAccessor.CodeRepository.UpdateCode(code);

                    return View("Place", code);
                }
                else if (Request["sample_id"] != null && Request["sample_id"].ToString() != "0")
                {
                    var sampleId = Convert.ToInt32(Request["sample_id"].ToString());
                    var fileName = string.Format("background_sample_{0}.jpg", sampleId);

                    code.BackgroundImage = FileHelper.GetFile(fileName);
                    code.BackgroundContentType = "image/jpeg";

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
            try
            {
                int x = (int)Math.Round(Convert.ToDouble(Request["x"].ToString()));
                int y = (int)Math.Round(Convert.ToDouble(Request["y"].ToString()));
                int width = (int)Math.Round(Convert.ToDouble(Request["width"].ToString()));
                int height = (int)Math.Round(Convert.ToDouble(Request["height"].ToString()));

                code.Rectangle = new Rectangle(x, y, width, height);
                code = DataAccessor.CodeRepository.UpdateCode(code);

                QrCodeHelper.CreateCode(code);
                QrCodeHelper.CreateThumbnail(code);

                if (!IsAuthenticated())
                {
                    CookieHelper.AddAnonymousCodeId(Request, Response, code.CodeId);
                }

                return RedirectToAction("Dashboard", "Generator");
            }
            catch (QRBaException ex)
            {
                Danger(ex.Message);
                return View(code);
            }
        }

        [HttpGet]
        public ActionResult Dashboard()
        {
            List<Code> codes = null;
            if (IsAuthenticated())
            {
                int accountId = GetAccountId();
                codes = DataAccessor.CodeRepository.GetCodes(accountId);
            }
            else
            {
                codes = new List<Code>();
                var codeIdList = CookieHelper.GetAnonymousCodeIdList(Request);
                foreach(var codeId in codeIdList)
                {
                    var code = DataAccessor.CodeRepository.GetCode(Constants.AnonymousId, codeId);
                    codes.Add(code);
                }
                
            }
            return View(codes);
        }

        [HttpGet]
        public ActionResult Download(int codeId)
        {
            if (!IsAuthenticated())
            {
                Danger("要登录才能下载您的透视码。");
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/Generator/Dashboard" });
            }

            var accountId = Convert.ToInt32(CookieHelper.GetCookie(Request, Constants.AccountId));
            //var code = DataAccessor.CodeRepository.GetCode(accountId, codeId);
            var data = FileHelper.GetCode(accountId, codeId);
            return File(data, Constants.ContentType);
        }
    }
}