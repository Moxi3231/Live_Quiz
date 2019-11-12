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
    public class GameController : Controller
    {
        // GET: Game
        [HttpGet]
        public ActionResult Index()
        {
            //test
            DataModel db = new DataModel();
            List<Quiz> lq;
            lq = db.Quizs.Where(q => q.isPublic == true).ToList();
            foreach (var q in lq)
            {
                if (!PinData.ht.ContainsValue(q.Id))
                {
                    PinData.ht.Add(PinData.pin, q.Id);
                    QuizPlayers.lu.Add(PinData.pin, new ArrayList());
                    UserAns.ans.Add(PinData.pin, new Hashtable());
                    Live.qon.Add(PinData.pin, "f");
                    UserAns.score.Add(PinData.pin++, new Hashtable());
                }
            }
            List<Question> qq = new List<Question>();
            Quiz quiz = db.Quizs.Find(PinData.ht[100]);
            foreach (QuizQuestion x in db.QuizQuestions.Include(q => q.Question).Include(q => q.Quiz).Where(x => x.QuizId == quiz.Id).ToList())
            {
                qq.Add(x.Question);
            }
            return View();
        }
        [ActionName("Index")]
        [HttpPost]
        public ActionResult check(FormCollection fc)
        {
            if (PinData.ht.ContainsKey(int.Parse(fc["pin"])))
            {
                Response.Redirect("game/nickname?pin=" + fc["pin"]);
            }
            return View("Index");
        }
        [HttpGet]
        public ActionResult nickname()
        {
            return View();
        }
        [ActionName("nickname")]
        [HttpPost]
        public ActionResult adduser(FormCollection fc)
        {
            Hashtable ua = (Hashtable)UserAns.ans[int.Parse(Request.QueryString["pin"])];
            Hashtable us = (Hashtable)UserAns.score[int.Parse(Request.QueryString["pin"])];
            Hashtable b = (Hashtable)UserAns.block;
            ArrayList ob = (ArrayList)QuizPlayers.lu[int.Parse(Request.QueryString["pin"])];
            Session["pin"] = int.Parse(Request.QueryString["pin"]);
            Session["name"] = fc["name"];
            Response.Cookies["pin"].Value = Request.QueryString["pin"];
            Response.Cookies["uname"].Value = fc["name"];
            us.Add(fc["name"], 0);
            ua.Add(fc["name"], "E");
            b.Add(fc["name"], "f");
            ob.Add(fc["name"]);
            return RedirectToAction("options");
        }
        [HttpPost]
        public void on(FormCollection fc)
        {
            Console.WriteLine(fc["btn"]);
            if (fc["btn"] == "Start")
            {
                Live.qon[int.Parse(fc["pin"])] = "t";
            }
            else
            {
                Live.qon[int.Parse(fc["pin"])] = "f";
            }
            Response.Redirect("Question?pin=" + fc["pin"]);
        }
        public ActionResult dashboard()
        {
            int pin = int.Parse(Request.QueryString["pin"]);
            ArrayList ob = (ArrayList)QuizPlayers.lu[pin];

            return View(ob);
        }
        [HttpGet]
        public ActionResult options()
        {
            return View();
        }
        [ActionName("options")]
        [HttpPost]
        public ActionResult BlockUser()
        {
            ArrayList ob = (ArrayList)QuizPlayers.lu[Session["pin"]];
            if (!ob.Contains(Session["name"]))
            {
                UserAns.block[Session["name"]] = "t";
            }
            return View("options");
        }

        public ActionResult live(string pin)
        {
            int p = int.Parse(pin);
            Live.qon[p] = "f";
            Hashtable uht = (Hashtable)UserAns.ans[p];
            Hashtable sht = (Hashtable)UserAns.score[p];
            sht.Cast<DictionaryEntry>().OrderBy(entry => entry.Value).ToList();
            return RedirectToAction("ScoreBoard", sht);
        }

        public ActionResult Question()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ScoreBoard()
        {
            Live.qon[int.Parse(Request.QueryString["pin"])] = "f";
            Hashtable uht = (Hashtable)UserAns.ans[int.Parse(Request.QueryString["pin"])];
            Hashtable sht = (Hashtable)UserAns.score[int.Parse(Request.QueryString["pin"])];
            sht.Cast<DictionaryEntry>().OrderBy(entry => entry.Value).ToList();
            return View(sht);
        }
        [ActionName("ScoreBoard")]
        [HttpPost]
        public ActionResult ScoreBoardP(FormCollection fc)
        {
            Hashtable sht = (Hashtable)UserAns.score[100];
            //sht["gmu"] = (int)sht["gmu"] + 1;
            if (fc["btn"] == "Next")
            {
                Live.qon[int.Parse(fc["pin"])] = "t";
                Response.Redirect("Question?pin=" + fc["pin"]);
            }
            return View("ScoreBoard", sht);
        }

        [HttpPost]
        public void DeleteUser()
        {
            UserAns.block[Request.Params["name"]] = "t";
            int pin = int.Parse(Request.QueryString["pin"]);
            Hashtable ua = (Hashtable)UserAns.ans[pin];
            ua.Remove(Request.Params["name"]);
            Hashtable us = (Hashtable)UserAns.ans[pin];
            us.Remove(Request.Params["name"]);
            ArrayList ob = (ArrayList)QuizPlayers.lu[pin];
            ob.Remove(Request.Params["name"]);
        }
    }
}