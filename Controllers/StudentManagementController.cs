using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StudentManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentManagementController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _context.StudentProfiles
                .Include(s => s.Branch)
                .Include(s => s.CurrentSemester)
                .Include(s => s.User)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Branches = await _context.Branches.ToListAsync();
            ViewBag.Semesters = await _context.Semesters.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentProfile profile, string email, string password)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _context.Branches.ToListAsync();
                ViewBag.Semesters = await _context.Semesters.ToListAsync();
                return View(profile);
            }

            // Create ApplicationUser
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = profile.Email
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                ViewBag.Branches = await _context.Branches.ToListAsync();
                ViewBag.Semesters = await _context.Semesters.ToListAsync();
                return View(profile);
            }

            await _userManager.AddToRoleAsync(user, "Student");

            profile.UserId = user.Id;
            profile.EnrollmentDate = DateTime.UtcNow;
            profile.CreatedAt = DateTime.UtcNow;

            _context.StudentProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.StudentProfiles.FindAsync(id);
            if (student == null) return NotFound();

            ViewBag.Branches = await _context.Branches.ToListAsync();
            ViewBag.Semesters = await _context.Semesters.ToListAsync();
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, StudentProfile profile)
        {
            if (id != profile.StudentProfileId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Branches = await _context.Branches.ToListAsync();
                ViewBag.Semesters = await _context.Semesters.ToListAsync();
                return View(profile);
            }

            try
            {
                var student = await _context.StudentProfiles.FindAsync(id);
                if (student == null) return NotFound();

                // Update only allowed fields to avoid accidentally changing the UserId (FK)
                student.RollNumber = profile.RollNumber;
                student.Email = profile.Email;
                student.PhoneNumber = profile.PhoneNumber;
                student.DateOfBirth = profile.DateOfBirth;
                student.IsActive = profile.IsActive;
                student.BranchId = profile.BranchId;
                student.CurrentSemesterId = profile.CurrentSemesterId;

                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentProfileExists(profile.StudentProfileId))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Deactivate(int id)
        {
            var student = await _context.StudentProfiles.FindAsync(id);
            if (student == null) return NotFound();

            student.IsActive = false;
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(student.UserId);
            if (user != null)
                user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);  // Permanently lock

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Activate(int id)
        {
            var student = await _context.StudentProfiles.FindAsync(id);
            if (student == null) return NotFound();

            student.IsActive = true;
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(student.UserId);
            if (user != null)
                user.LockoutEnd = null;

            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id) => _context.StudentProfiles.Any(e => e.StudentProfileId == id);
    }
}
