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
    public class StudentsController: Controller
    {
        private void InitSessionVariables()
        {
            if (Session["Search"] == null) Session["Search"] = false;
            if (Session["SearchString"] == null) Session["SearchString"] = "";
            if (Session["SelectedYear"] == null) Session["SelectedYear"] = "";
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetStudents(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                IEnumerable<Student> students = DB.Students.ToList();

                bool search = (bool)Session["Search"];
                string searchString = (string)Session["SearchString"];

                if (search)
                {
                    students = students.Where(s => (s.FirstName.ToLower() + " " + s.LastName.ToLower()).Contains(searchString));
                }

                string selectedYear = (string)Session["SelectedYear"];

                if (selectedYear != "")
                {
                    students = students.Where(s => (s.Year == selectedYear));
                }

                if (DB.Users.HasChanged || DB.Students.HasChanged || forceRefresh)
                {

                    return PartialView("GetStudents", students);
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
            var student = DB.Students.Get(id);

            return View(student);
        }

        public ActionResult GetStudentsDetails(int id, bool forceRefresh = false)
        {
            var student = DB.Students.Get(id);

            try
            {
                if (DB.Users.HasChanged || DB.Students.HasChanged || forceRefresh)
                {
                    InitSessionVariables();

                    return PartialView("GetStudentsDetails", student);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne" + ex.Message, "text/html");
            }
        }

        public ActionResult GetStudentsRegistrations(int id, bool forceRefresh = false)
        {
            var student = DB.Students.Get(id);

            try
            {
                if (DB.Users.HasChanged || DB.Students.HasChanged || forceRefresh)
                {
                    InitSessionVariables();

                    return PartialView("GetStudentsRegistrations", student);
                }
                return null;
            }
            catch (System.Exception ex)
            {
                return Content("Erreur interne" + ex.Message, "text/html");
            }
        }

        public ActionResult GetYearsList(bool forceRefresh = false)
        {
            try
            {
                InitSessionVariables();

                bool search = (bool)Session["Search"];

                if (search)
                {
                    return PartialView();
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
            if (Session["Search"] == null) Session["Search"] = false;
            Session["Search"] = !(bool)Session["Search"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchYear(string value)
        {
            Session["SelectedYear"] = value;
            return RedirectToAction("List");
        }
    }
}