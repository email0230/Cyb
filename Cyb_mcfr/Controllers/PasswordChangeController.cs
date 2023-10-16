using Cyb_mcfr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cyb_mcfr.Controllers
{
    [Authorize]
    public class PasswordChangeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        static int PasswordValidityDays = 30;

        public PasswordChangeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            EditUserModel model = new EditUserModel { Email = user.Email };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(IFormCollection collection)
        {
            var user = await _userManager.FindByEmailAsync(collection["Email"]);

            if (collection["NewPassword"].Equals(collection["NewPasswordConfirm"]) && await _userManager.CheckPasswordAsync(user, collection["Password"]))
            {
                foreach (var hash in user.PasswordHistory)
                {
                    var result = _userManager.PasswordHasher.VerifyHashedPassword(user, hash, collection["NewPassword"]);
                    if (result == PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Your new password was already used. Create another one.");
                        return View();
                    }
                }


                var pass = _userManager.PasswordHasher.HashPassword(user, collection["NewPassword"]);
                user.PasswordHash = pass;
                user.PasswordHistory = user.PasswordHistory.Append(user.PasswordHash).ToArray();
                user.PasswordValidity = DateTime.Now.AddDays(PasswordValidityDays);
                user.NeedToChangePassword = false;
                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.UpdateAsync(user);
                await _signInManager.SignOutAsync();
            }

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
