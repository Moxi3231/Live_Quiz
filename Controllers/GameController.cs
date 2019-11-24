using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;
using System.Collections;
using System.Threading;

namespace Live_Quiz.Controllers
{
    public class GameController : Controller
    {
        // GET: Game
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.err = false;
            //test
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
            foreach (DictionaryEntry pair in PinData.qql)
            {
                List<Question> qq = (List<Question>)pair.Value;
                if (qq.Count == 0)
                {
                    Quiz quiz = db.Quizs.Find(PinData.ht[pair.Key]);
                    foreach (QuizQuestion x in db.QuizQuestions.Include(q => q.Question).Include(q => q.Quiz).Where(x => x.QuizId == quiz.Id).ToList())
                    {
                        qq.Add(x.Question);
                    }
                }
                else
                {
                    qq.Clear();
                    Quiz quiz = db.Quizs.Find(PinData.ht[pair.Key]);
                    foreach (QuizQuestion x in db.QuizQuestions.Include(q => q.Question).Include(q => q.Quiz).Where(x => x.QuizId == quiz.Id).ToList())
                    {
                        qq.Add(x.Question);
                    }
                }
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
            ViewBag.err = true;
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
        public ActionResult dashboard(int? p)
        {
            int pin;
            if (p == null)
            {
                pin = int.Parse(Request.QueryString["pin"]);
            }
            else
            {
                pin = (int)p;
            }
            ArrayList ob = (ArrayList)QuizPlayers.lu[pin];

            return View(ob);
        }
        [HttpGet]
        public ActionResult options()
        {
            ViewBag.ua=(Hashtable)UserAns.score[int.Parse(Request.Cookies["pin"].Value.ToString())];
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
            DataModel db = new DataModel();
            lock (this)
            {
                List<Question> q = (List<Question>)PinData.qql[int.Parse(Request.QueryString["pin"])];
                Question ob;
             
                if (q.Count != 0)
                {
                    ob = q[0];
                    ViewBag.options = ob.Optionss;
                }
                else
                {
                    Live.qs[int.Parse(Request.QueryString["pin"])] = "f";
                    return View("End");
                }
                return View(ob);
            }
        }
        [HttpGet]
        public ActionResult ScoreBoard()
        { 
            Live.qon[int.Parse(Request.QueryString["pin"])] = "f";
            List<Question> q = (List<Question>)PinData.qql[int.Parse(Request.QueryString["pin"])];
            ICollection<Options> ol = q[0].Optionss;
            foreach(var i in ol)
            {
                if(i.isAnswer)
                {
                    ViewBag.qans = i.ans;
                }
            }
            Hashtable uht = (Hashtable)UserAns.ans[int.Parse(Request.QueryString["pin"])];
            Hashtable sht = (Hashtable)UserAns.score[int.Parse(Request.QueryString["pin"])];
            List<DictionaryEntry> d;
            d=sht.Cast<DictionaryEntry>().OrderByDescending(entry => entry.Value).ToList();
            return View(d);
        }
        [ActionName("ScoreBoard")]
        [HttpPost]
        public ActionResult ScoreBoardP(FormCollection fc)
        {
            lock (this)
            {
                List<Question> q = (List<Question>)PinData.qql[int.Parse(fc["pin"])];
                Hashtable sht = (Hashtable)UserAns.score[int.Parse(fc["pin"])];
               
                if (fc["btn"] == "Next")
                {
                    Live.qon[int.Parse(fc["pin"])] = "t";
                    Response.Redirect("Question?pin=" + fc["pin"]);
                }
                q.RemoveAt(0);
                return View("ScoreBoard", sht);
            }
        }

        [HttpPost]
        public void DeleteUser()
        {
            UserAns.block[Request.Params["name"]] = "t";
            int pin = int.Parse(Request.QueryString["pin"]);
            Hashtable ua = (Hashtable)UserAns.ans[pin];
            ua.Remove(Request.Params["name"]);
            Hashtable us = (Hashtable)UserAns.score[pin];
            us.Remove(Request.Params["name"]);
            ArrayList ob = (ArrayList)QuizPlayers.lu[pin];
            ob.Remove(Request.Params["name"]);
        }

        public ActionResult End()
        {
            return View();
        }

        public ActionResult FinalScore()
        {
            return View();
        }

        [AllowAnonymous][HttpPost]
        public void update_score()
        {
            int pin = int.Parse(Request.Cookies["pin"].Value.ToString());
            int ans = int.Parse(Request.Params["ans"]);
            Hashtable us = (Hashtable)UserAns.score[pin];
            List<Question> q = (List<Question>)PinData.qql[pin];
            ICollection<Options> ol = (ICollection<Options>)q[0].Optionss;
            if (Request.Params["ans"]!=null)
            {
                int k = 0;
                foreach(var i in ol)
                {
                    if(i.isAnswer)
                    {
                        if(k==ans)
                        {
                            us[Request.Cookies["uname"].Value.ToString()] = (int)us[Request.Cookies["uname"].Value.ToString()] + q[0].Score;
                        }
                        else
                        {
                            us[Request.Cookies["uname"].Value.ToString()] = (int)us[Request.Cookies["uname"].Value.ToString()] - q[0].Score;
                        }
                    }
                    k++;
                }
            }
        }
    }
}