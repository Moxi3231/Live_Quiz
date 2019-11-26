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
            Quiz quiz = db.Quizs.Where(x => x.Id == qid).FirstOrDefault();
            var res = db.QuizQuestions.Where(x => x.QuizId == qid).ToList();
            //var res = db.Questions.Select(x => x.QuestionId == qid).ToList();
            if(res.Count==0)
            {
                ViewBag.error = "Quiz Is Empty";
                return View("Error");
            }
            ViewBag.quiz = quiz;
            Session["one_quiz"] = quiz;
            int[] arr = new int[res.Count];
            for (int i = 0; i < res.Count; i++)
                arr[i] = 0;
            Session["question_list"] = arr;
            return View();
        }
        public ActionResult OneQuestion(FormCollection formCollection)
        {
            ViewBag.isValid = false;
            if(Session["one_quiz"]==null)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            int[] qlist = (int[])Session["question_list"];
            bool flag = true;
            bool flag2 = true;
            int ib = 0;
            foreach(int i in qlist)
            {
                if (i == 0)
                {
                    flag = false;
                }
                if (i == 1)
                {
                    flag2 = false;
                    ib++;
                }
            }
            if(flag)
            {
                return RedirectToAction("OnePlayerEnd");
            }
            if(flag2)
            {
                //logic for correct answer and pass to session
            }
            Quiz qui = (Quiz)Session["one_quiz"];
            var res = db.QuizQuestions.Where(x => x.QuizId == qui.Id).OrderByDescending(x=> x.QuestionId).ToList();
            if (res.Count == 0)
            {
                ViewBag.isValid = true;
                return View();
            }
            ViewBag.quiz = qui;
            
            qlist[ib] = 1;
            Session["question_list"] = qlist;
            int qiddd = res[ib].QuestionId;
            ViewBag.cquestion = db.Questions.Where(x=>x.QuestionId==qiddd).SingleOrDefault();

            return View();
        }
        public ActionResult OnePlayerEnd()
        {
            return View();
        }
    }
    
}