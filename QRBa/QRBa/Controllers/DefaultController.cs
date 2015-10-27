using log4net;
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
        private static readonly ILog logger = LogManager.GetLogger("QRBa");

        [HttpGet]
        [Route("{input}")]
        public HttpResponseMessage OnEvent(string input)
        {
            int accountId, codeId;
            UrlHelper.Code62Decode(input, out accountId, out codeId);
            string url = null;
            var code = DataAccessor.CodeRepository.GetCode(accountId, codeId);

            if (null != code)
            {
                var payload = JsonConvert.SerializeObject(
                    new
                    {
                        AccountId = accountId,
                        CodeId = codeId,
                        Type = EventType.Click,
                        ClientAddress = GetClientIp(),
                        UserAgent = Request.Headers.UserAgent.ToString(),
                    });

                logger.Info(payload);
                url = ((UrlPayload)code.Payload).TargetingUrl;
            }
            else
            {
                url = Constants.BaseUrl;
            }
            var response = Request.CreateResponse(HttpStatusCode.Moved);
            response.Headers.Location = url.GetUri();
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
