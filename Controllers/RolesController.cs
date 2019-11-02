using Live_Quiz.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Live_Quiz.Controllers
{
    [Authorize]
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
        [HttpGet]
        public ActionResult AssignRole(string role)
        {
            var allroles = roleManager.Roles.Select(x => x);
            ViewBag.roles = allroles.ToList();
            if (role==null)
            {
                ViewBag.isRoleSelected = false;
                return View();
            }
            ViewBag.isRoleSelected = true;
            ViewBag.role = role;
            ViewBag.rolename = allroles.SingleOrDefault(x => x.Id == role).Name;
            
            var selectedRole = roleManager.Roles.SingleOrDefault(x=>x.Id==role);

            //var users = (IEnumerable<ApplicationUser>)userManager.Users.Select(x => x ).ToList();
            //var users = userManager.Users.Select(x => x).ToList();
            List<ApplicationUser> userfinal = new List<ApplicationUser>();
            var usrf = userManager.Users.Select(x => x).ToList();
            var usr2 = context.Roles.SingleOrDefault(x => x.Id == role);
            var usr3 = usr2.Users.Select(x => x).ToList();

           
            
            foreach(var item in usrf)
            {
                var flag = false;
                usr3.ForEach(x => {
                    if(x.UserId==item.Id && flag==false)
                    {
                        userfinal.Add(item);
                        flag = true;
                    }
                });
            }
            List<ApplicationUser> userfinalL = new List<ApplicationUser>();
            usrf.ForEach(x => userfinalL.Add(x));
            userfinal.ForEach(x => userfinalL.Remove(x));
            ViewBag.Modal = (IEnumerable<ApplicationUser>)userfinalL;
            FormCollection coll = new FormCollection();
            coll["role"] = role;
            return View(coll);
        }
        [HttpPost]
        public ActionResult AssignRole(FormCollection collection)
        {
            //var user = userManager.FindById(collection["userid"]);
            userManager.AddToRole(collection["userid"], collection["role"]);
            return RedirectToAction("ViewUserRoles");
        }
        [HttpGet]
        public ActionResult ViewUserRoles()
        {
            var allusers = userManager.Users.Select(x => x);
            var allroles = roleManager.Roles.Select(x => x);
            ViewBag.roles = allroles.ToList();
            ViewBag.users = allusers.ToList();
            return View();
        }
    }
}