using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;
namespace Live_Quiz.Controllers
{
    public class OnePlayerModeController : Controller
    {
        DataModel db = new DataModel();
        // GET: OnePlayerMode
        public ActionResult Index()
        {
            ViewBag.error = "Access Denied";
            return View("Error");
        }
        public ActionResult OnePlayer(int? qid)
        {
           if(qid==null)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            var quiz = db.Quizs.Where(x => x.Id == qid).FirstOrDefault();
            var res = db.Questions.Select(x => x.QuestionId == qid).ToList();
            if(res.Count==0)
            {
                ViewBag.error = "Quiz Is Empty";
                return View("Error");
            }
            ViewBag.quiz = quiz;
            return View();
        }
        public ActionResult OneQuestion()
        {
            return View();
        }
    }
    
}