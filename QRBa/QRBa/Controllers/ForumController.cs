using QRBa.DataAccess;
using QRBa.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRBa.Controllers
{
    public class ForumController : BaseController
    {
        [Authorize]
        [HttpGet]
        // GET: Forum
        public ActionResult Index()
        {
            var list = DataAccessor.ForumRepository.GetComments();
            return View(list);
        }

        [HttpPost]
        public ActionResult AddComment()
        {
            int accountId = GetAccountId();
            string content = Request["content"].ToString();
            var comment = DataAccessor.ForumRepository.AddComment(
                new Comment
                {
                    AccountId = accountId,
                    Content = content,
                });
            return RedirectToAction("Index", "Forum");
        }
    }
}