namespace StudentPortal.Models
{
    public class Branch
    {
        public int BranchId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<StudentProfile> Students { get; set; } = new List<StudentProfile>();
        public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
