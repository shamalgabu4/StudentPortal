using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentDashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.User)
                .Include(s => s.Branch)
                .Include(s => s.CurrentSemester)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (studentProfile == null)
                return RedirectToAction("Login", "Account");

            var notices = await _context.Notices
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.StudentProfile = studentProfile;
            ViewBag.Notices = notices;

            return View();
        }

        public async Task<IActionResult> Results()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var studentProfile = await _context.StudentProfiles
                .Include(s => s.Branch)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (studentProfile == null)
                return RedirectToAction("Login", "Account");

            var marks = await _context.Marks
                .Where(m => m.StudentProfileId == studentProfile.StudentProfileId)
                .Include(m => m.Subject)
                .Include(m => m.Semester)
                .OrderBy(m => m.Semester.SemesterNumber)
                .ThenBy(m => m.Subject.Name)
                .ToListAsync();

            var semesters = await _context.Semesters.OrderBy(s => s.SemesterNumber).ToListAsync();
            ViewBag.Semesters = semesters;
            ViewBag.StudentProfile = studentProfile;

            return View(marks);
        }

        [HttpGet]
        public async Task<IActionResult> ViewMarks(int semesterId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (studentProfile == null)
                return RedirectToAction("Login", "Account");

            var marks = await _context.Marks
                .Where(m => m.StudentProfileId == studentProfile.StudentProfileId && m.SemesterId == semesterId)
                .Include(m => m.Subject)
                .Include(m => m.Semester)
                .ToListAsync();

            var semester = await _context.Semesters.FindAsync(semesterId);
            ViewBag.Semester = semester;
            ViewBag.TotalMarks = marks.Sum(m => m.ObtainedMarks);
            ViewBag.AverageMarks = marks.Count > 0 ? marks.Average(m => m.ObtainedMarks) : 0;

            return View(marks);
        }
    }
}
