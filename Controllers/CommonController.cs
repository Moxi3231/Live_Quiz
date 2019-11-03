using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;
namespace Live_Quiz.Controllers
{
    public class CommonController : Controller
    {
        private DataModel db = new DataModel();
        // GET: Common
        public ActionResult SearchQuizCollection(string value)
        {
            ViewBag.Searched_Value = value;
            ViewBag.isValAvail = false;
            if (value==null)
                return View();
            ViewBag.isValAvail = true;
            var collections = db;


            return View();
        }

    }
}