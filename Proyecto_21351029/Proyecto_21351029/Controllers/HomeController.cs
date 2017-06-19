using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_21351029.Controllers
{
    public class HomeController : Controller
    {
        ProyectoEntities db = new ProyectoEntities();

        public ActionResult Index()
        {
            User User = (from User2 in db.Users
                         select User2).FirstOrDefault();

            //Session["Admin"]
            //Session["User"] = User;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}