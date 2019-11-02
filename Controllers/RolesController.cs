using Live_Quiz.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Live_Quiz.Controllers
{

    public class RolesController : Controller
    {
        private static ApplicationDbContext context = new ApplicationDbContext();
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        // GET: Roles
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateRole()
        {
            ViewBag.flag = false;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateRole(Roles roles)
        {
            ViewBag.flag = false;
            if (!roleManager.RoleExists(roles.Name))
            {

                // first we create Admin rool    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = roles.Name;
                var res = roleManager.Create(role);
                ViewBag.message = "Role Created";
            }
            else
            {
                ViewBag.flag = true;
            }
            return View(roles);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ViewRoles()
        {
            return View((IEnumerable<Roles>)roleManager.Roles.Select(y => new Roles() { Id = y.Id, Name = y.Name }).ToList());
        }
        public ActionResult Delete(string id)
        {
            var res = roleManager.FindById(id);
            roleManager.Delete(res);
            return RedirectToAction("ViewRoles");
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditRole(string id)
        {
            var rres = roleManager.FindById(id);
            return View(new Roles() { Id = rres.Id, Name = rres.Name });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditRole(Roles roles)
        {
            ViewBag.message = "Updated!!!";
            var res = roleManager.FindById(roles.Id);
            res.Name = roles.Name;
            context.SaveChanges();
            //roleManager.Update(new IdentityRole() { Name=roles.Name,Id=roles.Id });
            return View(roles);
        }
    }
}