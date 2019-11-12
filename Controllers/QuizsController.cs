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
            ViewBag.quizid = id;
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
            List<Question> Qbank = new List<Question>();
            string idd = User.Identity.GetUserId();

            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));

            foreach (Question x in db.Questions.Where(x => x.User.Id==userPr.Id).ToList())
            {
                if (!(qq.Contains(x)))
                {
                    Qbank.Add(x);
                }
            }
            TempData["quizid"] = id;
            ViewBag.Qbank=Qbank;
            ViewBag.q = qq;
            return View(quiz);
        }
        [HttpGet]
        public ActionResult RemoveQuestionFromQuiz(int? qid, int? qizid)
        {
            if (qizid == null || qid == null)
            {
                return View("Error");
            }
            Quiz qiz = db.Quizs.Find(qizid);
            Question q = db.Questions.Find(qid);

            if (qiz == null || q == null)
            {
                return View("Error");
            }

            var quizque = qiz.QuizQuestions.Where(x => x.QuestionId == (int)qid).FirstOrDefault();
            //coll.QuizeCollections.Remove(quizcoll);
            db.QuizQuestions.Remove(quizque);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = qizid });
        }
        // GET: Quizs/Create
        public ActionResult Create()
        {

            var iserror = Convert.ToBoolean(Request.QueryString["isError"]);
            var error = Request.QueryString["error"];
            ViewBag.error = iserror;
            ViewBag.errormessage = error;
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
        public ActionResult Create(Quiz quiz)
        {
            var id = User.Identity.GetUserId();
            ViewBag.coll = db.Collections.Where(x => x.User.AccountId == id).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();

            HttpPostedFileBase file = Request.Files["ImageData"];
            //ContentRepository service = new ContentRepository();
            if (file == null)
            {
                ViewBag.noFile = "No file recieved.Please try again.";
                return View();
            }
            if (quiz.isPublic == true)
            {
                PinData.ht.Add(PinData.pin, quiz.Id);
                Live.qon.Add(PinData.pin, "f");
                QuizPlayers.lu.Add(PinData.pin, new ArrayList());
                UserAns.ans.Add(PinData.pin, new Hashtable());
                UserAns.score.Add(PinData.pin++, new Hashtable());
            }
            ContentRepository service = new ContentRepository();
            //ImageFile imageFile = new ImageFile();
            int i = service.UploadImageInDataBase(file, new ImageFielView() { });



            //if (i == 0)
            //{
             //   return View(quiz);
            //}


            if (ViewBag.coll.Count == 0)
            {
                return RedirectToAction("Create", "Collections", new { isError = true, error = "no collection, please create one." });
            }
            if (ModelState.IsValid)
            {
                string idd = User.Identity.GetUserId();
                UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));
                quiz.UPId = userPr.Id;
                userPr.Quizzes.Add(quiz);
                quiz.UPId = userPr.Id;
                quiz.ImageId = i;
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
            TempData.Keep("quizId");
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
            int quizid = int.Parse(TempData["quizid"].ToString());
            string idd = User.Identity.GetUserId();
            List<Question> qq = new List<Question>();
            foreach (QuizQuestion x in db.QuizQuestions.Include(q => q.Question).Include(q => q.Quiz).Where(x => x.QuizId == quizid).ToList())
            {
                qq.Add(x.Question);
            }
            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));
            List<Question> Qbank = new List<Question>();
            foreach (Question x in db.Questions.Where(x => x.User.Id == userPr.Id).ToList())
            {
                if (!(qq.Contains(x)))
                {
                    Qbank.Add(x);
                }
            }

            ViewBag.Qbank = Qbank;//db.Questions.Where(x => x.User.Id == userPr.Id).Select(y => new SelectListItem { Text = y.Description, Value = y.QuestionId.ToString() });

            return View();
        }
        [HttpPost]
        public ActionResult QuestionBank(FormCollection form)
        {
            TempData.Keep("quizId");
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
        public ActionResult Edit(Quiz quiz)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files["ImageData"];
                //ContentRepository service = new ContentRepository();

                int i = 0;
                //ImageFile imageFile = new ImageFile();
                if (file != null)

                {
                    ContentRepository service = new ContentRepository();
                    i = service.UploadImageInDataBase(file, new ImageFielView() { });
                    if (i == 0)
                    {
                        ViewBag.noFile = "Please try again.Couldn't upload file";
                        return View(quiz);
                    }
                    var imageData = db.Images.SingleOrDefault(x => x.Id == quiz.ImageId);
                    if (imageData != null)
                        db.Images.Remove(imageData);

                    db.SaveChanges();
                    //collection.ImageId = i;
                }
                bool saveFailed;
                do
                {
                    var qui = db.Quizs.SingleOrDefault(x => x.Id == quiz.Id);
                    qui.ImageId = i;
                    qui.isPublic = quiz.isPublic;
                    qui.Name = quiz.Name;

                    saveFailed = false;

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;

                        // Update the values of the entity that failed to save from the store
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(quiz);
                    }

                } while (saveFailed);
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
