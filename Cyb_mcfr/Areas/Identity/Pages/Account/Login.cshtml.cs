// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Cyb_mcfr.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Cyb_mcfr.Services;
using Cyb_mcfr.Interfaces;

namespace Cyb_mcfr.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IActivityService _activityService;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger, IActivityService activityService)
        {
            _signInManager = signInManager;
            _logger = logger;
            _activityService = activityService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    Activity a = new Activity { Username = Input.Email, Date = DateTime.Now, Action = "Login", Description = "User succesfully logged in" };
                    _activityService.AddAction(a);

                    var user = await _signInManager.UserManager.FindByEmailAsync(Input.Email);
                    bool isAdmin = await _signInManager.UserManager.IsInRoleAsync(user, "Admin");

                    if (!isAdmin)
                    {
                        returnUrl = "/Identity/Account/Manage";
                    }

                    if (user.NeedToChangePassword || user.PasswordValidity < DateTime.Now)
                    {
                        returnUrl = "/PasswordChange";
                    }

                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut) //TODO: replace this one with code that: shows the alert div in login.cshtml, starts a 5 minute timer serverside so it persists between refreshes
                {
                    ViewData["ShowLockoutDiv"] = true; ; // Set the flag to true
                    cTicker();
                    _logger.LogWarning("User account locked out.");
                    return Page();
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
                    return Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        public void cTicker()
        {
            // Calculate the timer's expiration time (e.g., 5 minutes from the current time)
            DateTime expirationTime = DateTime.Now.AddMinutes(5);

            // Store the expiration time in a session variable
            HttpContext.Session.SetString("TimerExpiration", expirationTime.ToString());

            string expirationTimeString = HttpContext.Session.GetString("TimerExpiration");
            if (expirationTimeString != null && DateTime.TryParse(expirationTimeString, out DateTime expirationTimeParsed))
            {
                TimeSpan remainingTime = expirationTimeParsed - DateTime.Now;
                int remainingSeconds = (int)remainingTime.TotalSeconds;

                // Pass the remaining time to the view
                ViewData["RemainingTimeInSeconds"] = remainingSeconds;
            }
        }
    }
}
