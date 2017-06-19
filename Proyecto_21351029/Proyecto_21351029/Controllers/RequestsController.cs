using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto_21351029;
using Proyecto_21351029.ViewModels;
using System.Globalization;

namespace Proyecto_21351029.Controllers
{
    public class RequestsController : Controller
    {
        private ProyectoEntities db = new ProyectoEntities();

        // GET: Requests
        public ActionResult Index()
        {
            return View(db.Requests.ToList());
        }

        // GET: Requests/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }
        
        // GET: Requests/Create
        public ActionResult Create()
        {
            var Classes = from TempClasses in db.Classes
                           select TempClasses;
            List<SelectListItem> ClassCodes = new List<SelectListItem>();

            foreach(var x in Classes)
            {
                ClassCodes.Add(new SelectListItem
                {
                    Text = x.class_name,
                    Value = x.class_code.ToString()
                });
            }

            ViewBag.ClassCodes = ClassCodes;
            return View();
        }
        
        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "request_date,class_code,hour")] Request request)
        {
            request.account_number = "21351029";
            int y = 0;
            var RequestsByUser = (from User in db.Requests
                                  where User.account_number == request.account_number
                                  select User);

            foreach (Request x in RequestsByUser)
            {
                if (GetWeekNumber(x.date_requested) == GetWeekNumber(DateTime.Today))
                {
                    y++;
                }
            }
            if (ModelState.IsValid)
            {
                if(y<3)
                {
                    TimeSpan duration = new TimeSpan(0, 12, 23, 3);

                    request.request_code = CreateCode(db.Requests.Count());
                    request.date_requested = DateTime.Today;
                    request.status = "Pending";
                    request.request_time = request.hour - request.date_requested;
                    request.hour = request.request_date + (request.hour - request.date_requested);

                    db.Requests.Add(request);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                if (y <= 3)
                {
                    TimeSpan duration = new TimeSpan(0, 12, 23, 3);

                    request.request_code = CreateCode(db.Requests.Count());
                    request.date_requested = DateTime.Today;
                    request.status = "Pending";
                    request.account_number = "21351029";
                    request.request_time = request.hour - request.date_requested;
                    request.hour = request.request_date + (request.hour - request.date_requested);

                    db.Requests.Add(request);
                    db.SaveChanges();
                }

                var Classes = from TempClasses in db.Classes
                              select TempClasses;
                List<SelectListItem> ClassCodes = new List<SelectListItem>();

                foreach (var x in Classes)
                {
                    ClassCodes.Add(new SelectListItem
                    {
                        Text = x.class_name,
                        Value = x.class_code.ToString()
                    });
                }

                ViewBag.ClassCodes = ClassCodes;
                return RedirectToAction("Index");
            }
        }
        
        public ActionResult Approve(string id)
        {
            Request Request = (from TempRequest in db.Requests
                               where TempRequest.request_code == id
                               select TempRequest).FirstOrDefault();

            Request.status = "Approved";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public ActionResult Reject(string id)
        {
            Request Request = (from TempRequest in db.Requests
                               where TempRequest.request_code == id
                               select TempRequest).FirstOrDefault();

            Request.status = "Rejected";
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        protected string CreateCode(int Amount)
        {
            if (Found("R-" + Amount))
            {
                return CreateCode(Amount + 1);
            }
            else
            {
                return "R-" + Amount;
            }
        }

        protected Boolean Found(string Code)
        {
            Request Request = (from TempRequest in db.Requests
                                 where TempRequest.class_code == Code
                                 select TempRequest).FirstOrDefault();
            if (Request != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public List<string> Classes()
        {
            var Classes = (from Class in db.Classes
                          select Class);
            List<string> class_codes = new List<string>();

            foreach(var x in Classes)
            {
                class_codes.Add(x.class_code);
            }
            return class_codes;
        }

        public ActionResult CreateTutorial()
        {
            return RedirectToAction("Create", "Tutorials");
        }

        public int GetWeekNumber(DateTime Date)
        {
            CultureInfo norwCulture = CultureInfo.CreateSpecificCulture("es");
            Calendar cal = norwCulture.Calendar;
            int weekNo = cal.GetWeekOfYear(Date,
                            norwCulture.DateTimeFormat.CalendarWeekRule,
                            norwCulture.DateTimeFormat.FirstDayOfWeek);
            return weekNo;
        }
    }
}
