using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Dashboard()
        {
            var totalStudents = await _context.StudentProfiles.CountAsync();
            var activeStudents = await _context.StudentProfiles.CountAsync(s => s.IsActive);
            var totalEnrollments = await _context.StudentSubjects.CountAsync();
            var totalFeedback = await _context.Feedbacks.CountAsync();
            var activeNotices = await _context.Notices.Where(n => n.IsActive).CountAsync();

            ViewBag.TotalStudents = totalStudents;
            ViewBag.ActiveStudents = activeStudents;
            ViewBag.TotalEnrollments = totalEnrollments;
            ViewBag.TotalFeedback = totalFeedback;
            ViewBag.ActiveNotices = activeNotices;

            // Recent notices
            var recentNotices = await _context.Notices.OrderByDescending(n => n.CreatedAt).Take(5).ToListAsync();
            return View(recentNotices);
        }

        [HttpGet]
        public IActionResult MarksEntry()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MarksEntry(int semesterId, int studentId, IFormCollection form)
        {
            var student = await _context.StudentProfiles.FindAsync(studentId);
            if (student == null) return NotFound();

            // Extract marks from dictionary (format: marks[subjectId])
            var semster = await _context.Semesters.FindAsync(semesterId);
            if (semster == null) return NotFound();

            foreach (var kvp in form)
            {
                // Only process marks fields: marks[1], marks[2], etc.
                if (!kvp.Key.StartsWith("marks[") || !kvp.Key.EndsWith("]"))
                    continue;

                // Extract subject ID from marks[subjectId]
                var subjectIdStr = kvp.Key.Substring(6, kvp.Key.Length - 7); // Remove "marks[" and "]"
                if (!int.TryParse(subjectIdStr, out var subjectId))
                    continue;

                if (!decimal.TryParse(kvp.Value, out var obtainedMarks))
                    continue;

                var subject = await _context.Subjects.FindAsync(subjectId);
                if (subject == null) continue;

                string grade = CalculateGrade(obtainedMarks);
                bool isPassed = obtainedMarks >= subject.PassingMarks;

                var existingMark = await _context.Marks
                    .FirstOrDefaultAsync(m => m.StudentProfileId == studentId && 
                                             m.SubjectId == subjectId && 
                                             m.SemesterId == semesterId);

                if (existingMark != null)
                {
                    existingMark.ObtainedMarks = obtainedMarks;
                    existingMark.Grade = grade;
                    existingMark.IsPassed = isPassed;
                    existingMark.UpdatedAt = DateTime.UtcNow;
                    _context.Update(existingMark);
                }
                else
                {
                    var newMark = new Marks
                    {
                        StudentProfileId = studentId,
                        SubjectId = subjectId,
                        SemesterId = semesterId,
                        ObtainedMarks = obtainedMarks,
                        Grade = grade,
                        IsPassed = isPassed,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Marks.Add(newMark);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Marks saved successfully!";
            return RedirectToAction(nameof(MarksEntry));
        }

        private string CalculateGrade(decimal marks)
        {
            if (marks >= 90) return "A+";
            if (marks >= 80) return "A";
            if (marks >= 70) return "B+";
            if (marks >= 60) return "B";
            if (marks >= 50) return "C+";
            if (marks >= 40) return "C";
            return "F";
        }
    }
}
