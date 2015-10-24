using Newtonsoft.Json;
using QRBa.DataAccess;
using QRBa.Domain;
using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace QRBa.Controllers
{
    [RoutePrefix("i")]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("{input}")]
        public HttpResponseMessage OnEvent(string input)
        {
            int accountId, codeId;
            UrlHelper.Code62Decode(input, out accountId, out codeId);
            var code = DataAccessor.CodeRepository.GetCode(accountId, codeId);

            var payload = JsonConvert.SerializeObject(
                new
                {
                    ClientAddress = GetClientIp(),
                    UserAgent = Request.Headers.UserAgent.ToString(),
                });
            DataAccessor.EventRepository.AddEvent(
                new Event
                {
                    AccountId = accountId,
                    CodeId = codeId,
                    Type = EventType.Click,
                    Payload = payload
                });

            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = new Uri(((UrlPayload)code.Payload).TargetingUrl);
            return response;
        }

        private string GetClientIp()
        {
            if (Request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }
    }
}
