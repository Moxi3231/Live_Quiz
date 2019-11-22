using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;
using System.Collections;


namespace Live_Quiz.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            DataModel db = new DataModel();
            List<Quiz> lq;
            lq = db.Quizs.Where(q => q.isPublic == true).ToList();
            foreach (var q in lq)
            {
                if (!PinData.ht.ContainsValue(q.Id))
                {
                    PinData.ht.Add(PinData.pin, q.Id);
                    PinData.qql.Add(PinData.pin, new List<Question>());
                    QuizPlayers.lu.Add(PinData.pin, new ArrayList());
                    UserAns.ans.Add(PinData.pin, new Hashtable());
                    Live.qon.Add(PinData.pin, "f");
                    Live.qs.Add(PinData.pin, "t");
                    UserAns.score.Add(PinData.pin++, new Hashtable());
                }
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}