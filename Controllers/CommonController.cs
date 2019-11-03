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


            List<Collection> collections = db.Collections.Where(x => (x.isPublic == false && x.Name.ToLower() == value.ToLower())).ToList();
            List<Quiz> quizs = db.Quizs.Where(x=>x.isPublic==false && x.Name.ToLower() == value.ToLower()).ToList();

            if(User.Identity.IsAuthenticated)
            {

            }
            return View();
        }

    }
}