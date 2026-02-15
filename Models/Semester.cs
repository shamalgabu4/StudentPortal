namespace StudentPortal.Models
{
    public class Semester
    {
        public int SemesterId { get; set; }
        public int SemesterNumber { get; set; } // 1-6
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsCommon { get; set; } // True for Semester 1 & 2 (common for all branches)
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
