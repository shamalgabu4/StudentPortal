using Microsoft.AspNetCore.Identity;

namespace StudentPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public virtual StudentProfile? StudentProfile { get; set; }
        public virtual ICollection<Notice> NoticesCreated { get; set; } = new List<Notice>();
    }
}
