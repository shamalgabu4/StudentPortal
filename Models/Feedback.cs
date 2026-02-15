using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Message { get; set; }

        public DateTime SubmittedOn { get; set; } = DateTime.UtcNow;
    }
}
