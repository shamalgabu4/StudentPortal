using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class MarksEntryViewModel
    {
        public int? BranchId { get; set; }
        public int? SemesterId { get; set; }
        public int? StudentProfileId { get; set; }

        [Display(Name = "Student")]
        public string? StudentName { get; set; }

        [Display(Name = "Roll Number")]
        public string? RollNumber { get; set; }

        public List<SubjectMarkViewModel> SubjectMarks { get; set; } = new();
    }

    public class SubjectMarkViewModel
    {
        public int SubjectId { get; set; }

        [Display(Name = "Subject")]
        public string? SubjectName { get; set; }

        [Display(Name = "Subject Code")]
        public string? SubjectCode { get; set; }

        [Display(Name = "Max Marks")]
        public int MaxMarks { get; set; }

        [Display(Name = "Passing Marks")]
        public int PassingMarks { get; set; }

        [Required(ErrorMessage = "Marks are required")]
        [Range(0, 100, ErrorMessage = "Marks must be between 0 and 100")]
        [Display(Name = "Obtained Marks")]
        public decimal ObtainedMarks { get; set; }
    }
}
