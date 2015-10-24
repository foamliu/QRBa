using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace QRBa
{
    public class ExceptionHandlingAttribute : HandleErrorAttribute
    {
        private static readonly ILog logger = LogManager.GetLogger("QRBa");

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext != null  && filterContext.Exception != null)
            {
                logger.FatalFormat("Fatal Error, message: {0}, stack trace: {1}.", filterContext.Exception.Message, filterContext.Exception.ToString());
            }

            base.OnException(filterContext);
        }
    }
}
