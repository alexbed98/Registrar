using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL;
using EmailHandling;
using Models;
using System.Web.Mvc;
using Registrar;
using static Controllers.AccessControl;

namespace Controllers
{
    public class TeachersController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["TeachersSearch"] == null) Session["TeachersSearch"] = false;
            if (Session["TeachersSearchString"] == null) Session["TeachersSearchString"] = "";
            if (Session["CurrentTeacherId"] == null) Session["CurrentTeacherId"] = 0;
        }

        private void ResetCurrentTeacherInfo()
        {
            Session["CurrentTeacherId"] = 0;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetTeachers(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                IEnumerable<Teacher> teachers = DB.Teachers.ToList().OrderBy(t => t.LastName);

                bool search = (bool)Session["TeachersSearch"];
                string searchString = (string)Session["TeachersSearchString"];

                if (search)
                {
                    teachers = teachers.Where(s =>
                         (s.FirstName.ToLower() + " " + s.LastName.ToLower())
                         .Contains(searchString.ToLower()));
                }

                if (DB.Users.HasChanged || DB.Teachers.HasChanged || forceRefresh)
                {
                    return PartialView("GetTeachers", teachers);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne" + ex.Message, "text/html");
            }
        }
        public ActionResult Details(int id)
        {
            var teacher = DB.Teachers.Get(id);

            Session["CurrentTeacherId"] = id;

            return View(teacher);
        }

        public ActionResult GetTeachersDetails(int id, bool forceRefresh = false)
        {
            var teacher = DB.Teachers.Get(id);

            try
            {
                if (DB.Users.HasChanged || DB.Teachers.HasChanged || forceRefresh)
                { 
                    return PartialView("GetTeachersDetails", teacher);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne" + ex.Message, "text/html");
            }
        }

        public ActionResult GetTeachersAllocations(int id, bool forceRefresh = false)
        {
            var teacher = DB.Teachers.Get(id);

            try
            {
                if (DB.Users.HasChanged || DB.Teachers.HasChanged || forceRefresh)
                {
                    return PartialView("GetTeachersAllocations", teacher);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne" + ex.Message, "text/html");
            }
        }

        public ActionResult ToggleSearch()
        {
            if (Session["TeachersSearch"] == null) Session["TeachersSearch"] = false;
            Session["TeachersSearch"] = !(bool)Session["TeachersSearch"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string value)
        {
            Session["TeachersSearchString"] = value;

            return RedirectToAction("List");
        }

        [UserAccess(Access.Admin)]
        public ActionResult Delete(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);

            if (teacher != null)
            {
                teacher.DeleteAllAllocations();
                teacher.DeleteNextSessionAllocations();

                DB.Events.Add("DeleteTeacher " + teacher.FullName);
                DB.Teachers.Delete(id);

                ResetCurrentTeacherInfo();

                return RedirectToAction("List");
            }

            return null;
        }

        [UserAccess(Access.Admin)]
        public ActionResult Edit()
        {
            int id = (int)Session["CurrentTeacherId"];
            Teacher teacher = DB.Teachers.Get(id);
            if (teacher != null)
            {
                //ViewBag.Registrations = teacher.NextSessionCoursesToSelectList;
                //ViewBag.Courses = DB.Courses.NextSessionToSelectList;
                return View(DB.Teachers.Get(id));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [UserAccess(Access.Admin)]
        public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId)
        {
            teacher.Id = (int)Session["CurrentTeacherId"];

            if (teacher.Id != 0)
            {
                //DB.Students.Update(student, selectedCoursesId);
                DB.Teachers.Update(teacher);
                return RedirectToAction("Details", new { id = teacher.Id });
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }

        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            return View(new Teacher());
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Teacher teacher)
        {
            int number = new Random().Next(0, 99999);

            teacher.Code = "CLG-420-" + number.ToString("D5");

            if (teacher.IsValid())
            {
                DB.Teachers.Add(teacher);
                return RedirectToAction("List");
            }
            DB.Events.Add("Illegal Create Teacher");
            return Redirect("/Accounts/Login?message=Erreur de creation de l'enseignant!&success=false");
        }
    }
}