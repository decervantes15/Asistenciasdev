using Asistencias.App_Start;
using Asistencias.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Asistencias.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public ActionResult Index(string ReturnUrl)
        {
            if (ReturnUrl != null) return RedirectToAction("Index");
            return View();
        }

        [HttpGet]
        public ActionResult Consulta()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Salir()
        {
            var context = Request.GetOwinContext();
            var auth = context.Authentication;

            auth.SignOut();

            return RedirectToAction("Index");
        }
    }
}