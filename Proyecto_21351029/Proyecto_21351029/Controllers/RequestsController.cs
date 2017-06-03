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
            
            RequestViewModel RequestView = new RequestViewModel()
            {
                class_codes = ClassCodes,

            };

            ViewBag.ClassCodes = ClassCodes;
            return View();
        }
        
        // POST: Requests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "request_date,class_code")] Request request)
        {
            if (ModelState.IsValid)
            {
                TimeSpan duration = new TimeSpan(0, 12, 23, 3);

                request.request_code = CreateCode(db.Requests.Count());
                request.date_requested = DateTime.Today;
                request.status = "Pending";
                request.account_number = "21351029";
                request.request_time = duration;

                db.Requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(request);
        }
        
        [HttpPost]
        public ActionResult CreateRequest([Bind(Include = "request_date,class_code")] Request Request)
        {
            Tutorial Tutorial = new Tutorial();
            TimeSpan duration = new TimeSpan(0, 12, 23, 3);

            Request.request_code = CreateCode(db.Requests.Count());
            Request.date_requested = DateTime.Today;
            Request.status = "Pending";
            Request.account_number = "21351029";
            Request.request_time = duration;

            /*
            Request Request = new Request()
            {
                request_code = CreateCode(db.Requests.Count()),
                account_number = "21351029",
                class_code = "C-021",
                date_requested = DateTime.Today,
                //request_date = model.,
                request_time = duration,
                status = "pending"
            };
            */
            db.Requests.Add(Request);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        
        // GET: Requests/Edit/5
        public ActionResult Edit(string id)
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

        // POST: Requests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "request_code,account_number,request_date,date_requested,status,class_code")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }

        // GET: Requests/Delete/5
        public ActionResult Delete(string id)
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

        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
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
            if (Found("T-" + Amount))
            {
                return CreateCode(Amount + 1);
            }
            else
            {
                return "T-" + Amount;
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
    }
}
