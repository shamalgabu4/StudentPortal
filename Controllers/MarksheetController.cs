using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class MarksheetController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
