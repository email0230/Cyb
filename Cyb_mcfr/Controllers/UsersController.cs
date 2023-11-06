using Cyb_mcfr.Data;
using Cyb_mcfr.Interfaces;
using Cyb_mcfr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cyb_mcfr.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IActivityService _activityService;

        public static int PasswordValidityDays = 30;
        public static int PasswordMinLength = 14;
        public static bool PasswordMustHaveDigits = true;
        public static int PasswordLockoutAttempts = 5;
        public static int SessionDurationMinutes = 5;

        public UsersController(ApplicationDbContext c, UserManager<ApplicationUser> userManager, IActivityService activity)
        {
            context = c;
            this.userManager = userManager;
            _activityService = activity;
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
            var user = new ApplicationUser();
            user.UserName = collection["email"];
            user.Email = collection["email"];
            user.EmailConfirmed = true;
            user.NeedToChangePassword = true;
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, collection["password"]);
            user.PasswordHistory = user.PasswordHistory.Append(user.PasswordHash).ToArray();

            await userManager.CreateAsync(user);
            //await userManager.AddToRoleAsync(user, "User");

            Activity a = new Activity { Username = User.Identity.Name, Date = DateTime.Now, Action = "Create user", Description = "User succesfully created " + user.Email };
            _activityService.AddAction(a);

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
        public async Task<ActionResult> Edit(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            EditUserModel model = new EditUserModel { Email = user.Email, Password = user.PasswordHash };

            return View(model);
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string email, IFormCollection collection)
        {
            var user = await userManager.FindByEmailAsync(email);
            
            if(collection["NewPassword"].Equals(collection["NewPasswordConfirm"]))
            {
                user.UserName = collection["NewEmail"];
                user.Email = collection["NewEmail"];
                user.NormalizedEmail = user.Email.Normalize();

                var pass = userManager.PasswordHasher.HashPassword(user, collection["NewPassword"]);
                user.PasswordHash = pass;
                user.PasswordHistory = user.PasswordHistory.Append(user.PasswordHash).ToArray();
                user.PasswordValidity = DateTime.Now.AddDays(PasswordValidityDays);
                //await userManager.ChangeEmailAsync(user, email, userManager.GenerateChangeEmailTokenAsync(user, email).Result);
                await userManager.UpdateAsync(user);

                Activity a = new Activity { Username = User.Identity.Name, Date = DateTime.Now, Action = "Edit user", Description = "User succesfully edited " + user.Email };
                _activityService.AddAction(a);
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

        // GET: UsersController/Delete/5
        public async Task<ActionResult> Delete(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            UserModel model = new UserModel { Email = user.Email };

            return View(model);
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string email, IFormCollection collection)
        {
            var user = await userManager.FindByEmailAsync(email);

            await userManager.DeleteAsync(user);


            Activity a = new Activity { Username = User.Identity.Name, Date = DateTime.Now, Action = "Delete user", Description = "User succesfully deleted " + user.Email };
            _activityService.AddAction(a);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Block(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            UserModel model = new UserModel { Email = user.Email, IsBlocked = user.isBlocked };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Block(string email, IFormCollection collection)
        {
            var user = await userManager.FindByEmailAsync(email);

            user.isBlocked = !user.isBlocked;

            if(user.isBlocked)
            {
                user.LockoutEnd = DateTime.MaxValue;
                Activity a = new Activity { Username = User.Identity.Name, Date = DateTime.Now, Action = "Block user", Description = "User succesfully blocked " + user.Email };
                _activityService.AddAction(a);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
                Activity a = new Activity { Username = User.Identity.Name, Date = DateTime.Now, Action = "Unblock user", Description = "User succesfully unblocked " + user.Email };
                _activityService.AddAction(a);
            }

            await userManager.UpdateAsync(user);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Rules()
        {
            var model = new EditUserModel { PassMinLength = PasswordMinLength, 
                PassMustHaveDigits = PasswordMustHaveDigits, 
                PassValidityDays = PasswordValidityDays, 
                PasswordLockoutAttempts = PasswordLockoutAttempts,
                SessionDuration = SessionDurationMinutes };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rules(EditUserModel model)
        {
            PasswordMinLength = model.PassMinLength;
            PasswordMustHaveDigits = model.PassMustHaveDigits;
            PasswordValidityDays = model.PassValidityDays;
            PasswordLockoutAttempts = model.PasswordLockoutAttempts;
            SessionDurationMinutes = model.SessionDuration;

            userManager.Options.Lockout.MaxFailedAccessAttempts = model.PasswordLockoutAttempts;

            Activity a = new Activity { Username = User.Identity.Name, Date = DateTime.Now, Action = "Change rules", Description = "User succesfully changed password rules" };
            _activityService.AddAction(a);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Activities()
        {
            var model = _activityService.GetAllActivities().ToList();

            return View(model);
        }

        public async Task<ActionResult> ToggleValidation(string email, IFormCollection collection)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);

            user.EnablePasswordValidation = !user.EnablePasswordValidation;
         
            await userManager.UpdateAsync(user);

            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> OneTimePassword(string email, IFormCollection collection)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(email);

            var x = Random.Shared.Next(1, 100);
            var a = user.UserName.Length;
            user.OneTimePasswordX = x;
            user.OneTimePasswordEnabled = true;

            var pass = Math.Log10(1.0 * a / x);
            if (pass < 0)
                pass *= -1;

            pass -= ((int)pass);
            pass *= 1000000;
            var passString = ((int)pass).ToString();

            user.OneTimePassword = userManager.PasswordHasher.HashPassword(user, passString);
         
            await userManager.UpdateAsync(user);

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
