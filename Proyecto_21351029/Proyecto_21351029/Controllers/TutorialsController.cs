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
using System.IO;

namespace Proyecto_21351029.Controllers
{
    public class TutorialsController : Controller
    {
        private ProyectoEntities db = new ProyectoEntities();

        // GET: Tutorials
        public ActionResult Index()
        {
            return View(db.Tutorials.ToList());
        }

        public ActionResult Export()
        {
            return View();
        }

        // GET: Tutorials/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            return View(tutorial);
        }

        // GET: Tutorials/Create
        public ActionResult Create()
        {
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
            return View();
        }
        
        // POST: Tutorials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tutorial_code,tutor_code,class_code,tutorial_date,tutorial_time")] Tutorial tutorial)
        {
            if (ModelState.IsValid)
            {
                tutorial.tutorial_code = CreateCode(db.Tutorials.Count());
                tutorial.student_amount = 0;
                db.Tutorials.Add(tutorial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tutorial);
        }

        // GET: Tutorials/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            return View(tutorial);
        }

        // POST: Tutorials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "tutorial_code,tutor_code,class_code,tutorial_date,tutorial_time")] Tutorial tutorial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tutorial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tutorial);
        }

        // GET: Tutorials/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            return View(tutorial);
        }

        // POST: Tutorials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Tutorial tutorial = db.Tutorials.Find(id);
            db.Tutorials.Remove(tutorial);
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

        public ActionResult CreateRequest()
        {
            return RedirectToAction("Create", "Requests");
        }

        protected string CreateCode(int Amount)
        {
            if(Found("T-" + Amount))
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
            Tutorial Tutorial = (from TempTutorial in db.Tutorials
                                 where TempTutorial.class_code == Code
                                 select TempTutorial).FirstOrDefault();
            if(Tutorial != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Subscribe(string id)
        {
            //agarrar el numero de cuenta
            Subcription Subscription = (from Subs in db.Subcriptions
                                        where Subs.account_number == "21351029" && Subs.tutorial_code == id
                                        select Subs).FirstOrDefault();

            if(Subscription == null)
            {
                Tutorial Tutorial = (from Tutorials in db.Tutorials
                                     where Tutorials.tutorial_code == id
                                     select Tutorials).FirstOrDefault();

                Tutorial.student_amount = Tutorial.student_amount + 1;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult ExportAmountOfTutorials()
        {
            var Tutorials = from Tutorial in db.Tutorials
                            select Tutorial;

            StreamWriter File = new StreamWriter("C:\\Users\\Edwin\\Documents\\SaveHere\\Text.csv");
            File.WriteLine("Tutorial Code, Amount, Date");
            foreach (var x in Tutorials)
            {
                User User = (from Users in db.Users
                             where Users.account_number == x.tutor_code
                             select Users).FirstOrDefault();

                Class Class = (from TempClass in db.Classes
                               where TempClass.class_code == x.class_code
                               select TempClass).FirstOrDefault();

                File.WriteLine(x.tutorial_code + "," + Class.class_name + "," + x.tutorial_date + "," + x.tutorial_time + ","
                                 + User.complete_name + "," + x.student_amount);
            }
            File.Flush();
            return RedirectToAction("Index");
        }

        public ActionResult ExportAfterDate(DateTime StartingDate)
        {
            var Tutorials = from Tutorial in db.Tutorials
                            where Tutorial.tutorial_date >= StartingDate
                            select Tutorial;

            StreamWriter File = new StreamWriter("C:\\Users\\Edwin\\Documents\\SaveHere\\Text.csv");
            File.WriteLine("Tutorial Code, Amount");
            foreach (var x in Tutorials)
            {
                File.WriteLine(x.tutorial_code + ", " + x.student_amount);
            }
            File.Flush();
            return View("Export");
        }

        public ActionResult ExportBetweenDate(DateTime StartingDate, DateTime EndDate)
        {
            var Tutorials = from Tutorial in db.Tutorials
                            where Tutorial.tutorial_date >= StartingDate && Tutorial.tutorial_date <= EndDate
                            select Tutorial;

            return View("Export");
        }
        public ActionResult ExportBeforeDate(DateTime EndDate)
        {
            var Tutorials = from Tutorial in db.Tutorials
                            where Tutorial.tutorial_date <= EndDate
                            select Tutorial;

            return View("Export");
        }
    }
}
