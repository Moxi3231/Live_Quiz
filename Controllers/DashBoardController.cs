using Live_Quiz.Models;
using System.Linq;
using System.Web.Mvc;

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