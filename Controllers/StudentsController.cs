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
        public ActionResult List()
        {
            return View();
        }
    }
}