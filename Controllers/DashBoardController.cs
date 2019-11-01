using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Live_Quiz.Models;

namespace Live_Quiz.Controllers
{
    public class DashBoardController : Controller
    {
        // GET: DashBoard
        public ActionResult Index()
        {
            DataModel db = new DataModel();
              db.Questions.ToList();
            return View();
        }
    }
}