namespace StudentPortal.Models
{
    public class StudentSubject
    {
        public int StudentSubjectId { get; set; }

        public int StudentProfileId { get; set; }
        public virtual StudentProfile? StudentProfile { get; set; }

        public int SubjectId { get; set; }
        public virtual Subject? Subject { get; set; }

        public int SemesterId { get; set; }
        public virtual Semester? Semester { get; set; }

        public DateTime EnrolledDate { get; set; }
    }
}
