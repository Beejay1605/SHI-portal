using Manager.Commons.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebPortalMVC.Models;

namespace WebPortalMVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IJwtTokenService tokenService;

        public HomeController(ILogger<HomeController> logger, IJwtTokenService tokenService)
        {
            _logger = logger;
            this.tokenService = tokenService;
        }

        public IActionResult Index()
        {
            //if (!tokenService.ValidatePage(HttpContext.Session.GetString(SecurityConst.TOKEN_KEY),
            //    AccessLevelEnum.Operations.ToString()))
            //{
            //    return Redirect("/Login");
            //}
            return Redirect("/Login");
            return View();
        }

        public IActionResult Products()
        {
            //if (!tokenService.ValidatePage(HttpContext.Session.GetString(SecurityConst.TOKEN_KEY),
            //    AccessLevelEnum.Operations.ToString()))
            //{
            //    return Redirect("/Login");
            //}
            return Redirect("/Login");
            return View();
        }
        public IActionResult ProductsDetails()
        {
            //if (!tokenService.ValidatePage(HttpContext.Session.GetString(SecurityConst.TOKEN_KEY),
            //    AccessLevelEnum.Operations.ToString()))
            //{
            //    return Redirect("/Login");
            //}
            return Redirect("/Login");
            return View();
        }
        public IActionResult PackageDetails()
        {
            return View();
        }

        public IActionResult Addtocart()
        {
            return View();
        }
        [Authorize]
        public IActionResult Privacy()
        {
            return Redirect("/Login");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
    }
}