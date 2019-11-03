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
        public ActionResult Index()
        {
            return RedirectToAction("SearchQuizCollection");
        }
        public ActionResult SearchQuizCollection(string value)
        {
            ViewBag.Searched_Value = value;
            ViewBag.isValAvail = false;
            if (value==null)
                return View();
            ViewBag.isValAvail = true;


            List<Collection> collections = db.Collections.Where(x => (x.isPublic == true && x.Name.ToLower() == value.ToLower())).ToList();
            List<Quiz> quizs = db.Quizs.Where(x=>x.isPublic==true && x.Name.ToLower() == value.ToLower()).ToList();

            ViewBag.collections = collections;
            ViewBag.quizs = quizs;
            return View();
        }
        public ActionResult ViewCollectionQuiz(int? CollectionId)
        {
            if(CollectionId==null)
            {
                ViewBag.error = "Access Denied.\n You are trying to access illegally";
                return View("Error");
            }
            Collection coll = db.Collections.SingleOrDefault(x => x.Id == CollectionId);
            ViewBag.Collection = coll.Name;
            if(coll.isPublic==false)
            {
                return HttpNotFound();
            }
            List<Quiz> reqQuiz = new List<Quiz>();
            var allQuiz = db.Quizs.Select(x => x).ToList();
            allQuiz.ForEach(x => {
                var tempflag = true;
            x.QuizCollections.ToList().ForEach(y=>{
                if(y.CollectionId == coll.Id && x.isPublic && tempflag)
                {
                    tempflag = false;
                    reqQuiz.Add(x);
                }
            });
            });
            ViewBag.quizs = reqQuiz;
            return View();
        }

    }
}