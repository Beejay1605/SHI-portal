using Microsoft.AspNetCore.Mvc;
using WebPortalMVC.Authorizations;

namespace WebPortalMVC.Areas.Operations.Controllers
{
    [Area("Operations")]
    public class DashboardController : Controller
    {
        
        //[AuthorizationView("Operations")]
        public async Task<IActionResult> Index()
        {
            ViewData["SidebarLocation"] = "Dashboard";
            return View();
        }
      
    }
}
