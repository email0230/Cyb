﻿using Cyb_mcfr.Interfaces;
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
        private readonly IActivityService _activityService;

        public PasswordChangeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, 
            IActivityService activity)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _activityService = activity;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            EditUserModel model = new EditUserModel { Email = user.Email, 
                PassMinLength = UsersController.PasswordMinLength,
                PassMustHaveDigits = UsersController.PasswordMustHaveDigits,
                PasswordValidation = user.EnablePasswordValidation
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(IFormCollection collection)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(collection["Email"]);

            EditUserModel model = new EditUserModel
            {
                Email = user.Email,
                PassMinLength = UsersController.PasswordMinLength,
                PassMustHaveDigits = UsersController.PasswordMustHaveDigits,
                PasswordValidation = user.EnablePasswordValidation
            };

            if (collection["NewPassword"].Equals(collection["NewPasswordConfirm"]) && await _userManager.CheckPasswordAsync(user, collection["Password"])) //TODO: remove checking if password is the same as the confirm password field
            {
                foreach (var hash in user.PasswordHistory)
                {
                    var result = _userManager.PasswordHasher.VerifyHashedPassword(user, hash, collection["NewPassword"]);
                    if (result == PasswordVerificationResult.Success)
                    {
                        ModelState.AddModelError(string.Empty, "Your new password was already used. Create another one.");

                        Activity ac = new Activity { Username = user.Email, Date = DateTime.Now, Action = "Change Password", Description = "User failed to change their password" };
                        _activityService.AddAction(ac);

                        return View(model);
                    }
                }


                var pass = _userManager.PasswordHasher.HashPassword(user, collection["NewPassword"]);
                user.PasswordHash = pass;
                user.PasswordHistory = user.PasswordHistory.Append(user.PasswordHash).ToArray();
                user.PasswordValidity = DateTime.Now.AddDays(UsersController.PasswordValidityDays);
                user.NeedToChangePassword = false;

                Activity a = new Activity { Username = user.Email, Date = DateTime.Now, Action = "Change Password", Description = "User changed their password successfully" };
                _activityService.AddAction(a);

                await _userManager.AddToRoleAsync(user, "User");
                await _userManager.UpdateAsync(user);
                await _signInManager.SignOutAsync();
            }
            else if(await _userManager.CheckPasswordAsync(user, collection["Password"]) == false)
            {
                ModelState.AddModelError(string.Empty, "Your current password is incorrect!");

                Activity ac = new Activity { Username = user.Email, Date = DateTime.Now, Action = "Change Password", Description = "User failed to change their password" };
                _activityService.AddAction(ac);

                return View(model);
            }

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(model);
            }
        }
    }
}
