using Newtonsoft.Json;
using QRBa.DataAccess;
using QRBa.Domain;
using QRBa.Models;
using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRBa.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        protected int GetAccountId()
        {
            var cookie = Request.Cookies[Constants.AccountId];
            Account account = null;

            if (cookie != null)
            {
                int accountId = Convert.ToInt32(cookie.Value);
                account = DataAccessor.AccountRepository.GetAccount(accountId);
                if (account != null && accountId == account.Id)
                    return accountId;
            }

            account = DataAccessor.AccountRepository.AddAccount(new Account { ClientInfo = GetClientInfo() });
            CookieHelper.SetCookie(Response, Constants.AccountId, account.Id.ToString(), true);
            return account.Id;
        }

        protected string GetClientInfo()
        {
            return JsonConvert.SerializeObject(new
            {
                UserAgent = Request.UserAgent.Truncate(400),
                UserHostAddress = Request.UserHostAddress
            });
        }

        public void Success(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Success, message, dismissable);
        }

        public void Information(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Information, message, dismissable);
        }

        public void Warning(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Warning, message, dismissable);
        }

        public void Danger(string message, bool dismissable = false)
        {
            AddAlert(AlertStyles.Danger, message, dismissable);
        }

        private void AddAlert(string alertStyle, string message, bool dismissable)
        {
            var alerts = TempData.ContainsKey(Alert.TempDataKey)
                ? (List<Alert>)TempData[Alert.TempDataKey]
                : new List<Alert>();

            alerts.Add(new Alert
            {
                AlertStyle = alertStyle,
                Message = message,
                Dismissable = dismissable
            });

            TempData[Alert.TempDataKey] = alerts;
        }

    }
}