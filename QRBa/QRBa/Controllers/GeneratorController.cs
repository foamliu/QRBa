using System;
using System.Collections.Generic;
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
        public ActionResult Select()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Place()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create()
        {
            return View();
        }


    }
}