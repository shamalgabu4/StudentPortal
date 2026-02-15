using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required]
        public string? Name { get; set; }
    }
}
