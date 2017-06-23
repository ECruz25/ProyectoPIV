using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto_21351029;

namespace Proyecto_21351029.Controllers
{
    public class ClassesController : Controller
    {
        private ProyectoEntities db = new ProyectoEntities();

        // GET: Classes
        public ActionResult Index()
        {
            User user = GetUser();

            if(user == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if(user.role == "Admin")
            {
                return View(db.Classes.ToList());
            }
            else
            {
                return RedirectToAction("UnableToAccess", "Users");
            }
        }

        // GET: Classes/Details/5
        public ActionResult Details(string id)
        {
            User user = GetUser();

            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (user.role == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Class @class = db.Classes.Find(id);
                if (@class == null)
                {
                    return HttpNotFound();
                }
                return View(@class);
            }
            else
            {
                return RedirectToAction("UnableToAccess", "Users");
            }
        }

        // GET: Classes/Create
        public ActionResult Create()
        {
            User user = GetUser();

            if (user == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else if (user.role == "Admin")
            {
                List<SelectListItem> Teachers = new List<SelectListItem>();

                var Users = from TempUser in db.Users
                            select TempUser;

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

                ViewBag.Teachers = Teachers;
                return View();
            }
            else
            {
                return RedirectToAction("UnableToAccess", "Users");
            }
            
        }

        // POST: Classes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "class_code,tutor_code,class_name")] Class @class)
        {
            if (ModelState.IsValid)
            {
                @class.class_code = CreateCode(db.Classes.Count());
                db.Classes.Add(@class);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@class);
        }

        // GET: Classes/Edit/5
        public ActionResult Edit(string id)
        {
            User user = GetUser();
            if(user == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                if (user.role == "Admin")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Class @class = db.Classes.Find(id);
                    if (@class == null)
                    {
                        return HttpNotFound();
                    }
                    return View(@class);
                }
                else
                {
                    return RedirectToAction("UnableToAccess", "Users");
                }
            }
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "class_code,tutor_code,class_name")] Class @class)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@class).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@class);
        }

        // GET: Classes/Delete/5
        public ActionResult Delete(string id)
        {
            User User = GetUser();
            if (User != null)
            {
                if(User.role == "Admin")
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    Class @class = db.Classes.Find(id);
                    if (@class == null)
                    {
                        return HttpNotFound();
                    }
                    return View(@class);
                }
                else
                {
                    return View("UnableToAccess", "Users");
                }
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Class @class = db.Classes.Find(id);
            db.Classes.Remove(@class);
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
            if (Found("C-" + Amount))
            {
                return CreateCode(Amount + 1);
            }
            else
            {
                return "C-" + Amount;
            }
        }

        protected Boolean Found(string Code)
        {
            Class Class = (from TempClass in db.Classes
                                 where TempClass.class_code == Code
                                 select TempClass).FirstOrDefault();
            if (Class != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected User GetUser()
        {
            return Session["User"] as User;
        }
    }
}
