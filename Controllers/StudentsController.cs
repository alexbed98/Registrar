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
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetStudents(bool forceRefresh = false)
        {
            try
            {
                var students = DB.Students.ToList();

                if (DB.Users.HasChanged ||
                    DB.Students.HasChanged ||
                    forceRefresh)
                {
                    InitSessionVariables();

                    return PartialView("GetStudents", students);
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