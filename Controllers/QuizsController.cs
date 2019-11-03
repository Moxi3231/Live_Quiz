using Live_Quiz.Models;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Live_Quiz.Controllers
{
    [Authorize]
    public class QuizsController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Quizs
        public ActionResult Index()
        {
            string idd = User.Identity.GetUserId();
            
            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));

            if (userPr.Quizzes == null)
            {
                userPr.Quizzes = new List<Quiz>();
            }
            return View(userPr.Quizzes.ToList());
        }

        // GET: Quizs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizs.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            ViewBag.questions = db.QuizQuestions.Include(q => q.Question).Include(q => q.Quiz).Where(x => x.QuizId == quiz.Id).ToList();
            List<Question> qq = new List<Question>();
            foreach (QuizQuestion x in db.QuizQuestions.Include(q => q.Question).Include(q => q.Quiz).Where(x => x.QuizId == quiz.Id).ToList())
            {
                qq.Add(x.Question);
            }
            ViewBag.q = qq;
            return View(quiz);
        }

        // GET: Quizs/Create
        public ActionResult Create()
        {
            var id = User.Identity.GetUserId();
            ViewBag.coll = db.Collections.Where(x=>x.User.AccountId==id).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            if(ViewBag.coll.Count ==0)
            {
                return RedirectToAction("Create", "Collections", new { isError = true, error = "no collection found, please create a collection in order to create to quiz." });
            }
            return View();
        }

        // POST: Quizs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,isPublic")] Quiz quiz)
        {
            var id = User.Identity.GetUserId();
            ViewBag.coll = db.Collections.Where(x => x.User.AccountId == id).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            if (ViewBag.coll.Count == 0)
            {
                return RedirectToAction("Create", "Collections", new { isError = true, error = "no collection, please create one." });
            }
            if (ModelState.IsValid)
            {
                db.Quizs.Add(quiz);
                int idc=int.Parse(Request.Form["coll"]);
               Collection cc= db.Collections.FirstOrDefault(x => x.Id == idc);
                QuizCollection qc = new QuizCollection
                {
                    Quiz = quiz,
                    Collection = cc
                };
                db.QuizCollections.Add(qc);
                db.SaveChanges();
                TempData["quizId"] = quiz.Id;
                 return RedirectToAction("AddAnother");
            }
           
            return View(quiz);
        }

        public ActionResult AddQuestions()
        {

            TempData.Keep("quizId");
            return View();
        }

        [HttpPost]
        public ActionResult AddQuestions(FormCollection form)
        {
            bool istrue;
            if (Request.Form["isPublic"].Contains("true"))
            {
                istrue = true;
            }
            else
            {
                istrue = false;
            }
            Question question = new Question
            {
                Description = Request.Form["Description"],
                Hint = Request.Form["Hint"],
                Score = int.Parse(Request.Form["Score"]),
                isPublic = istrue,
                Optionss = new List<Options>(),

            };

            if (ModelState.IsValid)
            {
                db.Questions.Add(question);


                bool iso1True = Request.Form["iso1True"] != null ? true : false;
                bool iso2True = Request.Form["iso2True"] != null ? true : false;
                bool iso3True = Request.Form["iso3True"] != null ? true : false;
                bool iso4True = Request.Form["iso4True"] != null ? true : false;

                Options o1 = new Options
                {
                    ans = Request.Form["o1ans"],
                    isAnswer = iso1True

                };
                Options o2 = new Options
                {
                    ans = Request.Form["o2ans"],
                    isAnswer = iso2True

                };
                Options o3 = new Options
                {
                    ans = Request.Form["o3ans"],
                    isAnswer = iso3True

                };
                Options o4 = new Options
                {
                    ans = Request.Form["o4ans"],
                    isAnswer = iso4True

                };

                question.Optionss.Add(o1);
                question.Optionss.Add(o2);
                question.Optionss.Add(o3);
                question.Optionss.Add(o4);

                string idd = User.Identity.GetUserId();
                UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));
                userPr.Questions.Add(question);
                int qid = (int)(TempData["quizId"]);
                TempData.Keep("quizId");
                Quiz qiz = db.Quizs.FirstOrDefault(x => x.Id == qid);
                
                userPr.Quizzes.Add(qiz);
                QuizQuestion qq = new QuizQuestion
                {
                    Question = question,
                    Quiz = qiz
                };
                qiz.QuizQuestions.Add(qq);
               // db.QuizQuestions.Add(qq);
                db.SaveChanges();
                return RedirectToAction("AddAnother");
            }
            return View();
        }
        public ActionResult AddAnother()
        {
            TempData.Keep("quizId");
            return View();
        }
        [HttpPost]
        public ActionResult AddAnother(FormCollection form)
        {
            TempData.Keep("quizId");
            if (Request.Form["Create"] != null)
            {
                return RedirectToAction("Index");
            }
            else if (Request.Form["QuestionBank"] != null)
            {
                return RedirectToAction("QuestionBank");
            }
            else
            {
                return RedirectToAction("AddQuestions");
            }
          
        }
        public ActionResult QuestionBank()
        {
            TempData.Keep("quizId");
            string idd = User.Identity.GetUserId();

            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));

            ViewBag.Qbank = db.Questions.Where(x => x.User.Id == userPr.Id).Select(y => new SelectListItem { Text = y.Description, Value = y.QuestionId.ToString() });

            return View();
        }
        [HttpPost]
        public ActionResult QuestionBank(FormCollection form)
        {
            int idd = int.Parse(Request.Form["Qbank"]);
            int qid = int.Parse(TempData["quizId"].ToString());
            QuizQuestion qq = new QuizQuestion
            {
                QuestionId = idd,
                QuizId = qid
            };
            db.QuizQuestions.Add(qq);
            db.SaveChanges();
            return RedirectToAction("AddAnother");
        }   // GET: Quizs/Edit/5
            public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizs.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: Quizs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,isPublic")] Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quiz).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(quiz);
        }

        // GET: Quizs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quiz quiz = db.Quizs.Find(id);
            if (quiz == null)
            {
                return HttpNotFound();
            }
            return View(quiz);
        }

        // POST: Quizs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quiz quiz = db.Quizs.Find(id);
            db.Quizs.Remove(quiz);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
