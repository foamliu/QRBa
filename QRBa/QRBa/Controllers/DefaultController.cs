﻿using QRBa.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
