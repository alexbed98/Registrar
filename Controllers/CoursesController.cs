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
        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetCourses(bool forceRefresh = false)
        {
            try
            {
                var courses = DB.Courses.ToList();

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

    }
}