using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Live_Quiz.Controllers
{
    public class CommonController : Controller
    {
         ApplicationDbContext context = new ApplicationDbContext();
        
        //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        private DataModel db = new DataModel();
        
        [NonAction]
        public void updateUserLeague(string id)
        {
            //DataModel db = new DataModel();
            int upid = db.UserProfiles.SingleOrDefault(x => x.AccountId == id).Id;

            var res = db.UserQuizzes.Where(x => x.UId == upid).ToList();
            int score = 0;
            foreach (var item in res)
            {
                score = score + item.Score;
            }
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var usr = UserManager.FindById(id);
            var leagues = db.Leagues.Select(x => x);
            foreach (var item in leagues)
            {
                if (item.Min_Value <= score && score <= item.Max_Value)
                {
                    usr.League = item.LeagueName;
                    break;
                }
            }
        }
        // GET: Common
        public ActionResult Index()
        {
            return RedirectToAction("SearchQuizCollection");
        }
        public ActionResult QuizBoard(int? quizid)
        {
            if(quizid==null)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            ViewBag.emFlag = false;
            ViewBag.qname = db.Quizs.SingleOrDefault(x => x.Id == quizid).Name;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var res = db.UserQuizzes.Where(x => x.QId == quizid).ToList().OrderByDescending(x => x.Score).ToList();
            int ti = 0;
            if(res.Count==0)
            {
                ViewBag.emFlag = true;
                return View();
            }
            List<UserQuizModal> rlist = new List<UserQuizModal>();
            foreach(UserQuiz uq in res)
            {
                ti++;
                if(ti==5)
                {
                    break;
                }
                string aid = db.UserProfiles.SingleOrDefault(x => x.Id == uq.UId).AccountId;
                var cuser = UserManager.FindById(aid);
                var uname = cuser.UserName;
                
                rlist.Add(new UserQuizModal() {uid=cuser.Id, score = uq.Score, Uname = uname,country=cuser.Country,league=cuser.League });
                
            }
            ViewBag.res = rlist;
            return View();
        }
        public ActionResult UserBoard(string uid)
        {
            if(uid==null)
            {
                ViewBag.error = "Access Denied";
                return View("Error");
            }
            ViewBag.emFlag = false;
            updateUserLeague(uid);
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var userr = UserManager.FindById(uid);

            ViewBag.uname = userr.UserName;
            ViewBag.league = userr.League;
            //var id = User.Identity.GetUserId();
            var user_profile = db.UserProfiles.SingleOrDefault(x => x.AccountId == uid);

            var res = db.UserQuizzes.Where(x => x.UId == user_profile.Id).ToList();
            if (res.Count == 0)
            {
                ViewBag.emFlag = true;
            }
            else
            {
                List<UserQuizView> rlist = new List<UserQuizView>();
                foreach (UserQuiz uq in res)
                {
                    var tempobj = db.Quizs.SingleOrDefault(x => x.Id == uq.UId);
                    if(tempobj==null)
                    {
                        ViewBag.error = "User hasn't played any quiz.";
                        return View("Error");
                    }
                    string qname = tempobj.Name;
                    rlist.Add(new UserQuizView() { QuizId = uq.QId, Score = uq.Score, QuizName = qname });

                }
                ViewBag.res = rlist;
            }
            return View();
            //return View();
        }
        [Authorize]
        public ActionResult ScoreBoard()
        {
            
            ViewBag.emFlag = false;
            var id = User.Identity.GetUserId();
            var user_profile = db.UserProfiles.SingleOrDefault(x => x.AccountId == id);

            var res = db.UserQuizzes.Where(x => x.UId==user_profile.Id).ToList();
            if(res.Count==0)
            {
                ViewBag.emFlag = true;
            }
            else
            {
                List<UserQuizView> rlist = new List<UserQuizView>();
               foreach(UserQuiz uq in res)
                {
                    var tempobj = db.Quizs.SingleOrDefault(x => x.Id == uq.UId);
                    if (tempobj == null)
                    {
                        ViewBag.error = "User hasn't played any quiz.";
                        return View("Error");
                    }
                    string qname = tempobj.Name;
                    rlist.Add(new UserQuizView() { QuizId = uq.QId, Score = uq.Score, QuizName = qname });

                }
                ViewBag.res = rlist;
            }
            return View();
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