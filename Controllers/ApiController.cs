using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;

namespace StudentPortal.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("branches")]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _context.Branches
                .Select(b => new { b.BranchId, b.Name })
                .ToListAsync();
            return Ok(branches);
        }

        [HttpGet("semesters")]
        public async Task<IActionResult> GetSemesters()
        {
            var semesters = await _context.Semesters
                .OrderBy(s => s.SemesterNumber)
                .Select(s => new { s.SemesterId, s.SemesterNumber })
                .ToListAsync();
            return Ok(semesters);
        }

        [HttpGet("students")]
        public async Task<IActionResult> GetStudents(int branchId, int semesterId)
        {
            var students = await _context.StudentProfiles
                .Where(s => s.BranchId == branchId && s.IsActive)
                .Include(s => s.User)
                .Select(s => new { s.StudentProfileId, s.RollNumber, FullName = s.User!.FullName, User = s.User! })
                .ToListAsync();
            return Ok(students);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudent(int studentId)
        {
            var student = await _context.StudentProfiles
                .Where(s => s.StudentProfileId == studentId)
                .Include(s => s.User)
                .Select(s => new { s.StudentProfileId, s.RollNumber, FullName = s.User!.FullName, User = s.User! })
                .FirstOrDefaultAsync();

            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpGet("subjects")]
        public async Task<IActionResult> GetSubjects(int semesterId, int branchId)
        {
            var subjects = await _context.Subjects
                .Where(s => s.SemesterId == semesterId && (s.BranchId == null || s.BranchId == branchId))
                .Select(s => new { 
                    s.SubjectId, 
                    s.Name, 
                    s.Code, 
                    s.MaxMarks, 
                    s.PassingMarks 
                })
                .ToListAsync();
            return Ok(subjects);
        }

        [HttpGet("marks")]
        public async Task<IActionResult> GetMarks(int studentId, int semesterId)
        {
            var marks = await _context.Marks
                .Where(m => m.StudentProfileId == studentId && m.SemesterId == semesterId)
                .Select(m => new { m.SubjectId, m.ObtainedMarks })
                .ToListAsync();
            return Ok(marks);
        }
    }
}
