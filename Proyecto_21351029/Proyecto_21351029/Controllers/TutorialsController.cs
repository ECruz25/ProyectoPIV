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

            User User = GetUser();

            if(User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (User.role == "Admin" || User.role == "TeacherAdmin")
            {
                return View(db.Tutorials.ToList());
            }
            else
            {
                return View("Index - ReadOnly", db.Tutorials.ToList());
            }
        }

        protected User GetUser()
        {
            return Session["User"] as User;
        }

        public ActionResult Export()
        {
            User User = GetUser();

            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (User.role == "Admin" || User.role == "TeacherAdmin")
            {
                return View();
            }
            else
            {
                return View("Index - ReadOnly", db.Tutorials.ToList());
            }
        }

        // GET: Tutorials/Details/5
        public ActionResult Details(int id)
        {
            Tutorial tutorial = db.Tutorials.Find(id);
            User User = GetUser();
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (User.role == "Admin" || User.role == "TeacherAdmin")
            {
                return View(tutorial);
            }
            else
            {
                return View("Index - ReadOnly", db.Tutorials.ToList());
            }
        }

        // GET: Tutorials/Create
        public ActionResult Create()
        {
            List<SelectListItem> ClassCodes = new List<SelectListItem>();
            List<SelectListItem> Teachers = new List<SelectListItem>();

            var Users = from TempUser in db.Users
                        select TempUser;

            var Classes = from TempClasses in db.Classes
                          select TempClasses;

            foreach(var Teacher in Users)
            {
                if(Teacher.role == "Teacher")
                {
                    Teachers.Add(new SelectListItem
                    {
                        Text = Teacher.complete_name,
                        Value = Teacher.account_number
                    });
                }
            }

            foreach (var x in Classes)
            {
                ClassCodes.Add(new SelectListItem
                {
                    Text = x.class_name,
                    Value = x.class_code.ToString()
                });
            }

            ViewBag.ClassCodes = ClassCodes;
            ViewBag.Teachers = Teachers;
            User User = GetUser();

            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (User.role == "Admin" || User.role == "TeacherAdmin")
            {
                return View();
            }
            else
            {
                return View("Index - ReadOnly", db.Tutorials.ToList());
            }
        }
        
        // POST: Tutorials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "tutor_code,class_code,tutorial_date,hour")] Tutorial tutorial)
        {
            if (ModelState.IsValid)
            {
                Class Class = (from TempClass in db.Classes
                               where TempClass.class_code == tutorial.class_code
                               select TempClass).FirstOrDefault();

                User User = (from TempUser in db.Users
                             where TempUser.account_number == tutorial.tutor_code
                             select TempUser).FirstOrDefault();

                DateTime Date = new DateTime(Convert.ToDateTime(tutorial.hour).Year, Convert.ToDateTime(tutorial.hour).Month, Convert.ToDateTime(tutorial.hour).Day);
                tutorial.id = CreateCode(db.Tutorials.Count());
                tutorial.student_amount = 0;
                tutorial.start_date = Convert.ToDateTime(tutorial.hour);
                tutorial.hour = tutorial.tutorial_date + (Convert.ToDateTime(tutorial.hour) - Date);
                db.Tutorials.Add(tutorial);
                tutorial.end_date = Convert.ToDateTime(tutorial.hour);
                tutorial.text = tutorial.id + "," + tutorial.tutor_code;
                tutorial.class_name = Class.class_name;
                tutorial.tutor_name = User.complete_name;
                db.SaveChanges();   
                return RedirectToAction("Index");   
            }
            else
            {
                List<SelectListItem> ClassCodes = new List<SelectListItem>();
                List<SelectListItem> Teachers = new List<SelectListItem>();

                var Users = from TempUser in db.Users
                            select TempUser;

                var Classes = from TempClasses in db.Classes
                              select TempClasses;

                foreach (var Teacher in Users)
                {
                    if (Teacher.role == "Teacher")
                    {
                        Teachers.Add(new SelectListItem
                        {
                            Text = Teacher.complete_name,
                            Value = Teacher.account_number
                        });
                    }
                }

                foreach (var x in Classes)
                {
                    ClassCodes.Add(new SelectListItem
                    {
                        Text = x.class_name,
                        Value = x.class_code.ToString()
                    });
                }

                ViewBag.ClassCodes = ClassCodes;
                ViewBag.Teachers = Teachers;
                User User = GetUser();

                if (User == null)
                {
                    return RedirectToAction("Login", "Users");
                }
                else if (User.role == "Admin" || User.role == "TeacherAdmin")
                {
                    return View();
                }
                else
                {
                    return View("Index - ReadOnly", db.Tutorials.ToList());
                }
            }
        }

        // GET: Tutorials/Edit/5
        public ActionResult Edit(int id)
        {
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            User User = GetUser();
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (User.role == "Admin" || User.role == "TeacherAdmin")
            {
                return View(tutorial);
            }
            else
            {
                return View("Index - ReadOnly", db.Tutorials.ToList());
            }
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
        public ActionResult Delete(int id)
        {
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
        public ActionResult DeleteConfirmed(int id)
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
            User User = GetUser();

            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                return RedirectToAction("Create", "Requests");
            }
        }

        protected int CreateCode(int Amount)
        {
            if(Found(Amount))
            {
                return CreateCode(Amount + 1);
            }
            else
            {
                return Amount;
            }
        }

        protected Boolean Found(int Code)
        {
            Tutorial Tutorial = (from TempTutorial in db.Tutorials
                                 where TempTutorial.id == Code
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

        public ActionResult Subscribe(int id)
        {
            User User = GetUser();

            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                Subcription Subscription = (from Subs in db.Subcriptions
                                            where Subs.account_number == "21351029" && Subs.tutorial_code == id.ToString()
                                            select Subs).FirstOrDefault();

                if (Subscription == null)
                {
                    Tutorial Tutorial = (from Tutorials in db.Tutorials
                                         where Tutorials.id == id
                                         select Tutorials).FirstOrDefault();

                    Tutorial.student_amount = Tutorial.student_amount + 1;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
        }

        public ActionResult ExportAmountOfTutorials()
        {

            User User = GetUser();

            if (User == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (User.role == "Admin" || User.role == "TeacherAdmin")
            {
                var Tutorials = from Tutorial in db.Tutorials
                                select Tutorial;

                StreamWriter File = new StreamWriter("Downloads\\Text.csv");
                File.WriteLine("Tutorial Code, Amount, Date");
                foreach (var x in Tutorials)
                {
                    User User2 = (from Users in db.Users
                                 where Users.account_number == x.tutor_code
                                 select Users).FirstOrDefault();

                    Class Class = (from TempClass in db.Classes
                                   where TempClass.class_code == x.class_code
                                   select TempClass).FirstOrDefault();

                    File.WriteLine(x.id + "," + Class.class_name + "," + x.tutorial_date + "," + x.start_date + ","
                                     + User2.complete_name + "," + x.student_amount);
                }
                File.Flush();
                File.Close();
                return RedirectToAction("Index");
            }
            else
            {
                return View("Index - ReadOnly", db.Tutorials.ToList());
            }
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
                File.WriteLine(x.id + ", " + x.student_amount);
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
