using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;

namespace Live_Quiz.Controllers
{
    public class LeaguesController : Controller
    {
        private DataModel db = new DataModel();

        // GET: Leagues
        public async Task<ActionResult> Index()
        {
            return View(await db.Leagues.ToListAsync());
        }

        // GET: Leagues/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = await db.Leagues.FindAsync(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // GET: Leagues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,LeagueName,Min_Value,Max_Value")] League league)
        {
            if (ModelState.IsValid)
            {
                db.Leagues.Add(league);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(league);
        }

        // GET: Leagues/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = await db.Leagues.FindAsync(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LeagueName,Min_Value,Max_Value")] League league)
        {
            if (ModelState.IsValid)
            {
                db.Entry(league).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(league);
        }

        // GET: Leagues/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = await db.Leagues.FindAsync(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            League league = await db.Leagues.FindAsync(id);
            db.Leagues.Remove(league);
            await db.SaveChangesAsync();
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
