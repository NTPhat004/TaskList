using Microsoft.AspNetCore.Mvc;

namespace TaskManagement.Controllers
{
    public class ActivityController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
