using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Students = await _context.Students.ToListAsync();
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Courses = await _context.Courses.ToListAsync();
                return View(enrollment);
            }

            // Check for duplicate enrollment (same student + same course)
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == enrollment.StudentId && e.CourseId == enrollment.CourseId);

            if (existingEnrollment != null)
            {
                var student = await _context.Students.FindAsync(enrollment.StudentId);
                var course = await _context.Courses.FindAsync(enrollment.CourseId);
                ModelState.AddModelError(string.Empty, $"Student '{student?.Name}' is already enrolled in '{course?.Name}'.");
                ViewBag.Students = await _context.Students.ToListAsync();
                ViewBag.Courses = await _context.Courses.ToListAsync();
                return View(enrollment);
            }

            enrollment.EnrolledOn = DateTime.UtcNow;
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Students");
        }
    }
}
