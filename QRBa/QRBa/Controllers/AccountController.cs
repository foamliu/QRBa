﻿using QRBa.DataAccess;
using QRBa.Domain;
using QRBa.Models;
using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace QRBa.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };

            return View("Login", model);
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = DataAccessor.AccountRepository.CheckIdentity(
                new Identity
                {
                    MemberName = model.MemberName,
                    PasswordHash = CryptoHelper.Hash(model.Password)
                });

            if (result)
            {
                FormsAuthentication.SetAuthCookie(model.MemberName, model.RememberMe);

                var account = DataAccessor.AccountRepository.GetAccountByIdentity(IdentityType.QRBaId, model.MemberName);
                CookieHelper.SetCookie(Response, Constants.AccountId, account.Id.ToString(), model.RememberMe);

                var codeIdList = CookieHelper.GetAnonymousCodeIdList(Request);
                foreach (var codeId in codeIdList)
                {
                    var code = DataAccessor.CodeRepository.GetCode(Constants.AnonymousId, codeId);
                    if (code != null)
                    {
                        code.AccountId = account.Id;
                        code.CodeId = 0;
                        code = DataAccessor.CodeRepository.AddCode(code);
                        DataAccessor.CodeRepository.RemoveCode(Constants.AnonymousId, codeId);
                        FileHelper.TransferCode(Constants.AnonymousId, codeId, account.Id, code.CodeId, code.BackgroundContentType);
                    }
                }
                CookieHelper.ClearAnonymousCodeIdList(Response);

                return RedirectToLocal(model.ReturnUrl);
            }
            else
            {
                Danger("用户名或密码错了 ╮(╯▽╰)╭", true);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var identity = DataAccessor.AccountRepository.AddIdentity(
                    new Identity
                    {
                        MemberName = model.MemberName,
                        PasswordHash = CryptoHelper.Hash(model.Password)
                    });

                var account = DataAccessor.AccountRepository.AddAccount(new Account { ClientInfo = GetClientInfo() });
                DataAccessor.AccountRepository.AddAccoutIdentity(account.Id, (byte)IdentityType.QRBaId, identity.MemberName);

                Success("注册成功, 请登录!", true);
                return View("Login");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Response.Cookies.Clear();

            FormsAuthentication.SignOut();

            HttpCookie c = new HttpCookie(Constants.AccountId);
            c.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(c);

            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult Manage()
        {
            int accountId = GetAccountId();
            var account = DataAccessor.AccountRepository.GetAccount(accountId);
            var model = new ManageAccountViewModel
            {
                Name = account.Name
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Manage(ManageAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                int accountId = GetAccountId();
                var account = DataAccessor.AccountRepository.UpdateAccount(
                    new Account
                    {
                        Id = accountId,
                        Name = model.Name
                    });
                Success("账户信息更新成功!", true);
                return View();
            }
            return View();
        }
    }
}