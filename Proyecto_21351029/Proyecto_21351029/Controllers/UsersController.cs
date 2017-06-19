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
    public class UsersController : Controller
    {
        private ProyectoEntities db = new ProyectoEntities();

        // GET: Users
        public ActionResult Index()
        {
            User User = GetUser();

            if(User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    return View(db.Users.ToList());
                }
            }
            else
            {
                return RedirectToAction("Login");   
            }
        }

        protected User GetUser()
        {
            return Session["User"] as User;
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            User User = GetUser();

            if (User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    User user = db.Users.Find(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            User User = GetUser();

            if (User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    List<SelectListItem> Roles = new List<SelectListItem>();
                    Roles.Add(new SelectListItem
                    {
                        Text = "Teacher",
                        Value = "Teacher"
                    });
                    Roles.Add(new SelectListItem
                    {
                        Text = "Student",
                        Value = "Student"
                    });
                    Roles.Add(new SelectListItem
                    {
                        Text = "Admin",
                        Value = "Admin"
                    });
                    Roles.Add(new SelectListItem
                    {
                        Text = "TeacherAdmin",
                        Value = "TeacherAdmin"
                    });

                    ViewBag.Roles = Roles;
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "account_number,complete_name,password,email,phone_number,role")] User user)
        {
            User User = GetUser();

            if (User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        db.Users.Add(user);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }

                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            User User = GetUser();

            if (User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    User user = db.Users.Find(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "account_number,complete_name,password,email,phone_number,role")] User user)
        {
            User User = GetUser();

            if (User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {

            User User = GetUser();

            if (User != null)
            {
                if (User.role != "Admin")
                {
                    return View("UnableToAccess");
                }
                else
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    User user = db.Users.Find(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    return View(user);
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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

        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login([Bind(Include = "account_number,password")] User user)
        {
            User UserTemp = (from Users in db.Users
                         where Users.account_number == user.account_number &&
                                Users.password == user.password
                         select Users).FirstOrDefault();
            if (UserTemp == null)
            {
                return View();
            }
            else
            {
                Session["User"] = UserTemp;
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UnableToAccess()
        {
            return View();
        }
    }
}
 