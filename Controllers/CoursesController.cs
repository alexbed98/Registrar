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
    public class CoursesController : Controller
    {
        private void InitSessionVariables()
        {
            if (Session["CoursesSearch"] == null) Session["CoursesSearch"] = false;
            if (Session["CoursesSearchString"] == null) Session["CoursesSearchString"] = "";
            if (Session["CurrentCourseId"] == null) Session["CurrentCourseId"] = 0;
        }

        private void ResetCurrentCourseInfo()
        {
            Session["CurrentCourseId"] = 0;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetCourses(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                IEnumerable<Course> courses = DB.Courses.ToList();

                bool search = (bool)Session["CoursesSearch"];
                string searchString = (string)Session["CoursesSearchString"];

                if (search)
                {
                    courses = courses.Where(c =>
                        c.Title.ToLower().Contains(searchString.ToLower())).ToList();
                }

                if (DB.Users.HasChanged || DB.Courses.HasChanged || forceRefresh)
                {
                    return PartialView("GetCourses", courses);
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
            var course = DB.Courses.Get(id);

            Session["CurrentCourseId"] = id;

            return View(course);
        }

        public ActionResult GetCoursesRegistrations(int id, bool forceRefresh = false)
        {
            var course = DB.Courses.Get(id);

            try
            {
                if (DB.Users.HasChanged || DB.Students.HasChanged || 
                    DB.Courses.HasChanged || DB.Teachers.HasChanged || 
                    forceRefresh)
                {
                    return PartialView("GetCoursesRegistrations", course);
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
            if (Session["CoursesSearch"] == null) Session["CoursesSearch"] = false;
            Session["CoursesSearch"] = !(bool)Session["CoursesSearch"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string value)
        {
            Session["CoursesSearchString"] = value;

            return RedirectToAction("List");
        }

        [UserAccess(Access.Admin)]
        public ActionResult Delete(int id)
        {
            Course course = DB.Courses.Get(id);

            if (course != null)
            {
                course.DeleteAllRegistrations();
                course.DeleteAllAllocations();

                DB.Events.Add("DeleteCourse " + course.Title);
                DB.Courses.Delete(id);

                ResetCurrentCourseInfo();

                return RedirectToAction("List");
            }

            return null;
        }

        [UserAccess(Access.Admin)]
        public ActionResult Edit()
        {
            int id = (int)Session["CurrentCourseId"];
            Course course = DB.Courses.Get(id);
            if (course != null)
            {
                ViewBag.Registrations = course.NextSessionStudentsToSelectList;
                ViewBag.Students = SelectListUtilities<Student>.Convert(
                    DB.Students.ToList().OrderBy(c => c.Code).ToList(), "Caption");
                return View(DB.Courses.Get(id));
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [UserAccess(Access.Admin)]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            course.Id = (int)Session["CurrentCourseId"];

            if (course.Id != 0)
            { 
                DB.Courses.Update(course);
                course.UpdateRegistrations(selectedStudentsId);
                return RedirectToAction("Details", new { id = course.Id });
            }
            return Redirect("/Accounts/Login?message=Accès illégal! &success=false");
        }

        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            ViewBag.Students = SelectListUtilities<Student>.Convert(
                    DB.Students.ToList().OrderBy(c => c.Code).ToList(), "Caption");

            return View(new Course());
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Course course, List<int> selectedStudentsId)
        {
            if (course.IsValid())
            {
                DB.Courses.Add(course);
                course.UpdateRegistrations(selectedStudentsId);
                return RedirectToAction("List");
            }
            DB.Events.Add("Illegal Create Course");
            return Redirect("/Accounts/Login?message=Erreur de creation du cours!&success=false");
        }

    }
}