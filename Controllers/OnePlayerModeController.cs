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
            Session["anslist"] = arr;
            Session["one_score"] = 0;
            Session["one_quiz_valid"] = true;
            Session["one_point"] = 0;
            return View();
        }
        public ActionResult OneQuestion(FormCollection formCollection)
        {
            bool ftempflag = Convert.ToBoolean(Session["one_quiz_valid"]);
            if(!ftempflag)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            ViewBag.isValid = false;
            if(Session["one_quiz"]==null)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            int[] qlist = (int[])Session["question_list"];
            int[] anslist = (int[])Session["anslist"];
            bool flag = true;
            bool flag2 = false;
            int ib = Convert.ToInt32(Session["one_point"]);
            foreach(int i in qlist)
            {
                if (i == 0)
                {
                    flag = false;
                }
                if (i == 1)
                {
                    flag2 = true;
                    //ib++;
                }
            }
            //var ttttttt = ib;
            Quiz qui = (Quiz)Session["one_quiz"];
            var res = db.QuizQuestions.Where(x => x.QuizId == qui.Id).OrderByDescending(x => x.QuestionId).ToList();
            if (flag || ib==res.Count)
            {
                int stmm = Convert.ToInt32(Request.QueryString["score"]);
                if(stmm==0)
                {
                    anslist[ib-1] = 0;
                }
                else
                {
                    anslist[ib-1] = 1;
                }
                int sco = (int)Session["one_score"];
                sco += stmm;
                Session["one_score"] = sco;
                Session["isValidEnd"] = true;
                return RedirectToAction("OnePlayerEnd");
            }
            if(flag2)
            {
                //logic for correct answer and pass to session
                int stmm = Convert.ToInt32(Request.QueryString["score"]);
                if (stmm == 0)
                {
                    anslist[ib-1] = 0;
                }
                else
                {
                    anslist[ib-1] = 1;
                }
                int sco = (int)Session["one_score"];
                sco += stmm;
                Session["one_score"] = sco;

            }
            Session["anslist"] = anslist;
            
            var trescount = res.Count;
            if (res.Count == 0)
            {
                ViewBag.isValid = true;
                return View();
            }
            ViewBag.quiz = qui;
            
            qlist[ib] = 1;
            Session["question_list"] = qlist;
            int qiddd = res.ElementAt(ib).QuestionId;
            //int qiddd = res[ib].QuestionId;
            ViewBag.cquestion = db.Questions.Where(x=>x.QuestionId==qiddd).SingleOrDefault();
            Session["one_point"] = ib+1;
            return View();
        }
        public ActionResult OnePlayerEnd()
        {
            Session["one_quiz_valid"] = false;
            bool flag = Convert.ToBoolean(Session["isValidEnd"]);
            if(!flag)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            Session["isValidEnd"] = false;
            int sco = (int)Session["one_score"];
            ViewBag.tscore = sco;
            int[] anslist = (int[])Session["anslist"];
            ViewBag.anslist = anslist;
            
            return View();
        }
    }
    
}