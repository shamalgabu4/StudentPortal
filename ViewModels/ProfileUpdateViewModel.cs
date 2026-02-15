using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class ProfileUpdateViewModel
    {
        public string? UserId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full Name must be between 2 and 100 characters")]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Profile Picture URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ProfilePictureUrl { get; set; }
    }
}
