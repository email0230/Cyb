using Cyb_mcfr.Interfaces;
using Cyb_mcfr.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cyb_mcfr.Controllers
{
    public class OneTimePasswordController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IActivityService _activityService;

        public OneTimePasswordController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IActivityService activity)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _activityService = activity;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(IFormCollection collection)
        {
            var user = await _userManager.FindByEmailAsync(collection["Email"]);

            if(user == null || !user.OneTimePasswordEnabled)
            {
                ModelState.AddModelError(string.Empty, "This account does not have one-time password set.");
                return View();
            }

            UserModel model = new UserModel { Email = user.Email };

            return RedirectToAction("Login", model);
        }

        public async Task<ActionResult> Login(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var model = new UserModel { Email = user.Email };
            ViewData["x"] = user.OneTimePasswordX;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(UserModel userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.Email);

            if(userModel.Password == null)
            {
                userModel.Password = string.Empty;
            }

            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.OneTimePassword, userModel.Password);

            if (result == PasswordVerificationResult.Success)
            {
                await _signInManager.SignInAsync(user, true);
                user.OneTimePassword = string.Empty;
                user.OneTimePasswordX = 0;
                user.OneTimePasswordEnabled = false;
                await _userManager.UpdateAsync(user);

                Activity a = new Activity { Username = user.Email, Date = DateTime.Now, Action = "Login", Description = "User succesfully logged in using one-time password" };
                _activityService.AddAction(a);

                return RedirectToAction("Index", "Home");
            }

            Activity ac = new Activity { Username = user.Email, Date = DateTime.Now, Action = "Login", Description = "User failed to login using one-time password" };
            _activityService.AddAction(ac);

            ModelState.AddModelError(string.Empty, "One-Time Password is invalid.");
            ViewData["x"] = user.OneTimePasswordX;
            return View(userModel);
        }
    }
}
