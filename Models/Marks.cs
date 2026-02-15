namespace StudentPortal.Models
{
    public class Marks
    {
        public int MarksId { get; set; }

        public int StudentProfileId { get; set; }
        public virtual StudentProfile? StudentProfile { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject? Subject { get; set; }

        public int SemesterId { get; set; }
        public virtual Semester? Semester { get; set; }

        public decimal ObtainedMarks { get; set; }
        public string Grade { get; set; } = string.Empty;
        public bool IsPassed { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
