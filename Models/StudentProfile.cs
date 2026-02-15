namespace StudentPortal.Models
{
    public class StudentProfile
    {
        public int StudentProfileId { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser? User { get; set; }

        public string RollNumber { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; } = true;

        public int BranchId { get; set; }
        public virtual Branch? Branch { get; set; }

        public int CurrentSemesterId { get; set; }
        public virtual Semester? CurrentSemester { get; set; }

        public DateTime EnrollmentDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        public virtual ICollection<Marks> Marks { get; set; } = new List<Marks>();
    }
}
