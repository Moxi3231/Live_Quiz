using Live_Quiz.Models;
using Live_Quiz.Models.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace Live_Quiz.Controllers
{
    [Authorize]

    public class CollectionsController : Controller
    {
        private static ApplicationDbContext context = new ApplicationDbContext();

        UserManager<ApplicationUser> UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        private DataModel db = new DataModel();

        // GET: Collections
        public ActionResult Index()
        {
            string idd = User.Identity.GetUserId();
            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId == idd);

            var data = userPr.Collections.ToList() ;
            if(data==null)
            {
                
                return RedirectToAction("Create",new {isError=true,error="No Collection, Create One" });
            }
            return View(data as IEnumerable<Collection>);
        }

        // GET: Collections/Details/5
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
            allQuizes.ForEach(b=> {
                var tempflag = false;
                b.QuizCollections.Select(a => a).ToList().ForEach(n=>
                { 
                    if(n.CollectionId==id && tempflag==false)
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
            var uis = User.Identity.GetUserId();
            var aquizzes = db.UserProfiles.FirstOrDefault(x => x.AccountId ==uis ).Quizzes.Select(x => x).ToList();

            aquizzes.ForEach(b=> {
                var flag = true;
                b.QuizCollections.Select(x => x).ToList().ForEach(a =>
                {
                    if(a.CollectionId==id && flag)
                    {
                        flag = false;
                        availableQuizzes.Add(b);
                        
                    }
                });
            });
            aquizzes.ForEach(x => {
                if(!availableQuizzes.Contains(x))
                {
                    availableQuizzes2.Add(x);
                }
            });
            ViewBag.availQuiz = availableQuizzes2;
            ViewBag.collectionid = id;
            return View(collection);
        }

        // GET: Collections/Create
        public ActionResult Create()
        {
            var iserror = Convert.ToBoolean( Request.QueryString["isError"]);
            var error = Request.QueryString["error"];
            ViewBag.error = iserror;
            ViewBag.errormessage = error;
            //ViewBag.email = UserManager.FindById(User.Identity.GetUserId()).Email;
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Collection collection)
        {
            if (!ModelState.IsValid)
            {
                return View(collection);
            }

            HttpPostedFileBase file = Request.Files["ImageData"];
            //ContentRepository service = new ContentRepository();
            if (file==null)
            {
                ViewBag.noFile = "No file recieved.Please try again.";
                return View();
            }
            ContentRepository service = new ContentRepository();
            //ImageFile imageFile = new ImageFile();
            int i = service.UploadImageInDataBase(file,new ImageFielView() { });
            
          
             
            if (i == 0)
            {
                return View(collection);
            }
           
            string idd = User.Identity.GetUserId();

            UserProfile userPr = db.UserProfiles.FirstOrDefault(x => x.AccountId.Equals(idd));

            collection.Email = UserManager.FindById(idd).Email;
            collection.User = userPr;
            collection.UserProfileId = userPr.Id;
            collection.User = userPr;
            collection.ImageId = i;
            db.Collections.Add(collection);
            db.SaveChanges();
            //db.SaveChanges();
            //return View(collection);
            return RedirectToAction("Index");

        }

        // GET: Collections/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection collection = db.Collections.Find(id);
            if (collection == null)
            {
                return HttpNotFound();
            }
            return View(collection);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Collection collection)
        {
            if (ModelState.IsValid)
            {

                HttpPostedFileBase file = Request.Files["ImageData"];
                //ContentRepository service = new ContentRepository();

                int i=0;
                //ImageFile imageFile = new ImageFile();
                if (file != null)
                
                {
                    ContentRepository service = new ContentRepository();
                    i = service.UploadImageInDataBase(file, new ImageFielView() { });
                    if (i == 0)
                    {
                        ViewBag.noFile = "Please try again.Couldn't upload file";
                        return View(collection);
                    }
                    db.Images.Remove(db.Images.Single(x => x.Id == collection.ImageId));
                    db.SaveChanges();
                    //collection.ImageId = i;
                } 
                //db.Entry(collection).State = EntityState.Modified;
                bool saveFailed;
                do
                {
                    var coll = db.Collections.Single(x => x.Id == collection.Id);
                    coll.ImageId = i;
                    coll.isPublic = collection.isPublic;
                    coll.Name = collection.Name;
                    
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
                        entry.OriginalValues.SetValues(collection);
                    }

                } while (saveFailed);
                
                return RedirectToAction("Index");
            }
            return View(collection);
        }

        // GET: Collections/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Collection collection = db.Collections.Find(id);
            if (collection == null)
            {
                return HttpNotFound();
            }
            return View(collection);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Collection collection = db.Collections.Find(id);
            db.Collections.Remove(collection);
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
        [HttpGet]
        [AllowAnonymous]
        public ActionResult RetrieveImage(int id)
        {
            byte[] cover = GetImageFromDataBase(id);
            if (cover != null)
            {
                return File(cover, "image/jpg");
            }
            else
            {
                return HttpNotFound();
            }
        }
        [AllowAnonymous]
        public byte[] GetImageFromDataBase(int Id)
        {
            var q = (from temp in db.Images where temp.Id == Id select temp.Image).FirstOrDefault();
            if (q == null)
            {

                return null;
            }
            byte[] cover = q;
            return cover;
        }
        [HttpPost]
        public ActionResult AddQuizToCollection(FormCollection model)
        {
            var cid1 = model["collectionid"];
            var qid1 = model["quizid"];
            if (cid1 == null || qid1 == null)
            {
                return View("Error");
            }
            var cid= Convert.ToInt64(cid1.ToString());
            var qid = Convert.ToInt64(qid1.ToString());
            Collection coll = db.Collections.Find(cid);
            Quiz quiz = db.Quizs.Find(qid);

            if (coll == null || quiz == null)
            {
                return View("Error");
            }
            var qc = new QuizCollection() {CollectionId=(int)cid,QuizId=(int)qid,Collection=coll,Quiz=quiz };
            db.QuizCollections.Add(qc);
            db.SaveChanges();
            return RedirectToAction("Details",new { id=cid});
        }
        [HttpGet]
        public ActionResult RemoveQuizFromCollection(int? qid, int? cid)
        {
            if (cid == null || qid == null)
            {
                return View("Error");
            }
            Collection coll = db.Collections.Find(cid);
            Quiz quiz = db.Quizs.Find(qid);
            
            if(coll==null || quiz==null)
            {
                return View("Error");
            }

            var quizcoll = coll.QuizeCollections.Where(x => x.QuizId == (int)qid).FirstOrDefault();
            //coll.QuizeCollections.Remove(quizcoll);
            db.QuizCollections.Remove(quizcoll);
            db.SaveChanges();
            return RedirectToAction("Details",new { id=cid});
        }
    }
}
