using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MarksManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarksManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? semesterId)
        {
            var semesters = await _context.Semesters.ToListAsync();
            ViewBag.Semesters = semesters;
            ViewBag.SelectedSemesterId = semesterId;

            if (!semesterId.HasValue)
                return View(new List<Marks>());

            var sid = semesterId.Value;

            var marks = await _context.Marks
                .Where(m => m.SemesterId == sid)
                .Include(m => m.StudentProfile)
                    .ThenInclude(s => s.User)
                .Include(m => m.Subject)
                .OrderBy(m => m.StudentProfile!.RollNumber)
                .ToListAsync();

            return View(marks);
        }

        [HttpGet]
        public async Task<IActionResult> EnterMarks(int semesterId, int studentId)
        {
            var semester = await _context.Semesters.FindAsync(semesterId);
            var student = await _context.StudentProfiles
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StudentProfileId == studentId);

            if (semester == null || student == null)
                return NotFound();

            var subjects = await _context.Subjects
                .Where(s => s.SemesterId == semesterId && (s.BranchId == null || s.BranchId == student.BranchId))
                .ToListAsync();

            var marks = await _context.Marks
                .Where(m => m.StudentProfileId == studentId && m.SemesterId == semesterId)
                .ToListAsync();

            ViewBag.Semester = semester;
            ViewBag.Student = student;
            ViewBag.Subjects = subjects;
            ViewBag.ExistingMarks = marks.ToDictionary(m => m.SubjectId);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveMarks(int semesterId, int studentId, Dictionary<int, decimal> marks)
        {
            var student = await _context.StudentProfiles.FindAsync(studentId);
            if (student == null) return NotFound();

            foreach (var kvp in marks)
            {
                var subjectId = kvp.Key;
                var obtainedMarks = kvp.Value;

                var subject = await _context.Subjects.FindAsync(subjectId);
                if (subject == null) continue;

                var existingMark = await _context.Marks
                    .FirstOrDefaultAsync(m => m.StudentProfileId == studentId && m.SubjectId == subjectId && m.SemesterId == semesterId);

                string grade = CalculateGrade(obtainedMarks);
                bool isPassed = obtainedMarks >= subject.PassingMarks;

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
            return RedirectToAction(nameof(Index), new { semesterId });
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
