using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            
            // Redirect based on role
            if (User.IsInRole("Admin"))
                return RedirectToAction("Dashboard", "Admin");
            
            if (User.IsInRole("Student"))
                return RedirectToAction("Index", "StudentDashboard");

            // If user is authenticated but has no recognized role, redirect to logout
            if (User.Identity?.IsAuthenticated == true)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            // Fallback for unauthenticated
            return RedirectToAction("Login", "Account");
        }

        [AllowAnonymous]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Contact()
        {
            return View();
        }
    }
}
