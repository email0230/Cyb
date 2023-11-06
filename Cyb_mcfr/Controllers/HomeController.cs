using Cyb_mcfr.Interfaces;
using Cyb_mcfr.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cyb_mcfr.Controllers
{
    [Authorize(Roles = "User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IActivityService _activityService;

        public HomeController(ILogger<HomeController> logger, IActivityService activity)
        {
            _logger = logger;
            _activityService = activity;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ActionResult Activities()
        {
            List<Models.Activity> model = new List<Models.Activity>();

            if(User.Identity.Name != null)
                model = _activityService.GetActivitiesForUser(User.Identity.Name).ToList();

            return View(model);
        }
    }
}