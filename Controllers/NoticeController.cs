using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;

namespace StudentPortal.Controllers
{
    [Authorize]
    public class NoticeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NoticeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var notices = await _context.Notices
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(notices);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Manage()
        {
            var notices = await _context.Notices
                .Include(n => n.CreatedByUser)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
            return View(notices);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Notice notice)
        {
            if (!ModelState.IsValid)
                return View(notice);

            var user = await _userManager.GetUserAsync(User);
            notice.CreatedByUserId = user!.Id;
            notice.CreatedAt = DateTime.UtcNow;

            _context.Notices.Add(notice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice == null) return NotFound();
            return View(notice);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Notice notice)
        {
            if (id != notice.NoticeId) return NotFound();

            if (!ModelState.IsValid)
                return View(notice);

            notice.UpdatedAt = DateTime.UtcNow;
            _context.Update(notice);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Manage));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice != null)
            {
                _context.Notices.Remove(notice);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Manage));
        }
    }
}
