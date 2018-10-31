using OracleCodeFirst.Web.CustomAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OracleCodeFirst.Web.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        [CustomAuthorize]
        public ActionResult Index()
        {
            ViewBag.Message = "You are logged.";
            return View();
        }

        [CustomAuthorize(Roles = "HO")]
        public ActionResult HO()
        {
            ViewBag.Message = "Only HeadOffice User.";
            return View();
        }

        [CustomAuthorize(Roles = "GMO")]
        public ActionResult GMO()
        {
            ViewBag.Message = "Only GMO User.";
            return View();
        }

        [CustomAuthorize(Roles = "PORO")]
        public ActionResult PORO()
        {
            ViewBag.Message = "Only PORO User.";
            return View();
        }
    }
}