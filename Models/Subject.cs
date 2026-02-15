namespace StudentPortal.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int Credits { get; set; }
        public int MaxMarks { get; set; } = 100;
        public int PassingMarks { get; set; } = 35;

        public int SemesterId { get; set; }
        public virtual Semester? Semester { get; set; }

        public int? BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public virtual ICollection<Marks> Marks { get; set; } = new List<Marks>();
    }
}
