using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
