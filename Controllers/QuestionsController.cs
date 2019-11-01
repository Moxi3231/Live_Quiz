using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;
using Microsoft.AspNet.Identity;

namespace Live_Quiz.Controllers
{
    public class QuestionsController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Questions
        public ActionResult Index()
        {
            string idd = User.Identity.GetUserId();

            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));


            return View(userPr.Questions.ToList());

        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            ViewBag.options = question.Optionss;

            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormCollection fomr)
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
                Score = int.Parse( Request.Form["Score"]),
                isPublic = istrue,
                Optionss=new List<Options>(),
                
            };

            if (ModelState.IsValid)
            {
                db.Questions.Add(question);


                bool iso1True = Request.Form["iso1True"]!=null? true : false;
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


                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(question);
        }

        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "QuestionId,Description,Score,Hint,isPublic")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
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
