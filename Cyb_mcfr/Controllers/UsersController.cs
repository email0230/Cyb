using Cyb_mcfr.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cyb_mcfr.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext context;
        UserManager<IdentityUser> userManager;

        public UsersController(ApplicationDbContext c, UserManager<IdentityUser> userManager)
        {
            context = c;
            this.userManager = userManager;
        }


        // GET: UsersController
        public ActionResult Index()
        {
            //var model = userManager.Users.ToList();
            var model = context.Users.ToList();

            return View(model);
        }

        // GET: UsersController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(IFormCollection collection)
        {
            var user = new IdentityUser();
            user.UserName = collection["email"];
            user.Email = collection["email"];
            user.EmailConfirmed = true;

            await userManager.CreateAsync(user, collection["password"]);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
