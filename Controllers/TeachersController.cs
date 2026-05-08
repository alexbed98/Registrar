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
    }
}