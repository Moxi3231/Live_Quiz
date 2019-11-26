using Live_Quiz.Models;
using Live_Quiz.Models.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Collections;
namespace Live_Quiz.Controllers
{
    public class ExploreController : Controller
    {
        // GET: Explore
        DataModel db = new DataModel();
        public ActionResult Index()
        {
          
            ViewBag.Collection = db.Collections.Where(x => x.isPublic == true).ToList();
            ViewBag.TopQUiz = db.Quizs.Where(x => x.isPublic == true).ToList();
            return View();
        }
        public ActionResult Info(int? id)
        {
            Quiz quiz = db.Quizs.FirstOrDefault(x => x.Id == id);
 
            ViewBag.quiz = quiz;
            return View();
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            Collection collection = db.Collections.Find(id);

            var allQuizes = db.Quizs.Select(x => x).ToList();
            List<Quiz> quizList = new List<Quiz>();
            /* allQuizes.ForEach(x => {
                if(x.Collection.Id==id)
                 {
                     quizList.Add(x);
                 }
             });*/
            allQuizes.ForEach(b => {
                var tempflag = false;
                b.QuizCollections.Select(a => a).ToList().ForEach(n =>
                {
                    if (n.CollectionId == id && tempflag == false)
                    {
                        tempflag = true;
                        quizList.Add(b);
                    }
                });
            });
            ViewBag.allQuizes = allQuizes;
            ViewBag.Quizes = quizList;
            if (collection == null)
            {
                return HttpNotFound();
            }


            List<Quiz> availableQuizzes = new List<Quiz>();
            List<Quiz> availableQuizzes2 = new List<Quiz>();
            var aquizzes = db.UserProfiles.FirstOrDefault(x =>true).Quizzes.Select(x => x).ToList();

            aquizzes.ForEach(b => {
                var flag = true;
                b.QuizCollections.Select(x => x).ToList().ForEach(a =>
                {
                    if (a.CollectionId == id && flag)
                    {
                        flag = false;
                        availableQuizzes.Add(b);

                    }
                });
            });
            aquizzes.ForEach(x => {
                if (!availableQuizzes.Contains(x))
                {
                    availableQuizzes2.Add(x);
                }
            });
            ViewBag.availQuiz = availableQuizzes2;
            ViewBag.collectionid = id;
            return View(collection);
        }
        public ActionResult Play(int? id)
        {
            /*Hashtable h=PinData.ht;

            Hashtable h=PinData.ht;
            foreach(DictionaryEntry e in h)
            {
                if((int)e.Value==id)
                {
                    return RedirectToAction("dashboard","game",new {pin=e.Key });
                }
            }*/
            PinData.ht.Add(PinData.pin, id);
            PinData.qql.Add(PinData.pin, new List<Question>());
            QuizPlayers.lu.Add(PinData.pin, new ArrayList());
            UserAns.ans.Add(PinData.pin, new Hashtable());
            UserAns.block.Add(PinData.pin, new Hashtable());
            Live.qon.Add(PinData.pin, "f");
            Live.qs.Add(PinData.pin, "t");
            UserAns.score.Add(PinData.pin++, new Hashtable());
            return RedirectToAction("dashboard", "game", new { pin = PinData.pin - 1 });
        }
    }
}