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
            if (Session["StudentsSearch"] == null) Session["StudentsSearch"] = false;
            if (Session["StudentsSearchString"] == null) Session["StudentsSearchString"] = "";
            if (Session["StudentsSelectedYear"] == null) Session["StudentsSelectedYear"] = "";
            if (Session["CurrentStudentId"] == null) Session["CurrentStudentId"] = 0;
        }

        private void ResetCurrentStudentInfo()
        {
            Session["CurrentStudentId"] = 0;
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

                bool search = (bool)Session["StudentsSearch"];
                string searchString = (string)Session["StudentsSearchString"];

                if (search)
                {
                    students = students.Where(s => 
                        (s.FirstName.ToLower() + " " + s.LastName.ToLower())
                        .Contains(searchString.ToLower()));
                }

                string selectedYear = (string)Session["StudentsSelectedYear"];

                if (selectedYear != "")
                {
                    students = students.Where(s => (s.Year == int.Parse(selectedYear)));
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

            Session["CurrentStudentId"] = id;

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

                bool search = (bool)Session["StudentsSearch"];

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
            if (Session["StudentsSearch"] == null) Session["StudentsSearch"] = false;
            Session["StudentsSearch"] = !(bool)Session["StudentsSearch"];
            return RedirectToAction("List");
        }

        public ActionResult SetSearchYear(string value)
        {
            Session["StudentsSelectedYear"] = value;
            return RedirectToAction("List");
        }

        public ActionResult SetSearchString(string value)
        {
            Session["StudentsSearchString"] = value;

            return RedirectToAction("List");
        }

        public ActionResult SetYear()
        {
            ViewBag.Year = NextSession.Year;
            ViewBag.Session = NextSession.ValidSessions.Contains(1) ? "Automne" : "Hiver";
            return View();
        }

        [HttpPost]
        public ActionResult SetYear(int year, string session)
        {
            NextSession.CurrentDate = new DateTime(year, (session == "Automne" ? 8 : 1), 15);
            return RedirectToAction("List");
        }
    }
}