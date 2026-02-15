using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Legacy
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        // New ERP Entities
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<Marks> Marks { get; set; }
        public DbSet<Notice> Notices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // StudentProfile relationships
            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.User)
                .WithOne(u => u.StudentProfile)
                .HasForeignKey<StudentProfile>(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.Branch)
                .WithMany(b => b.Students)
                .HasForeignKey(sp => sp.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.CurrentSemester)
                .WithMany()
                .HasForeignKey(sp => sp.CurrentSemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentSubject relationships
            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.StudentProfile)
                .WithMany(sp => sp.StudentSubjects)
                .HasForeignKey(ss => ss.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Semester)
                .WithMany()
                .HasForeignKey(ss => ss.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Marks relationships
            modelBuilder.Entity<Marks>()
                .HasOne(m => m.StudentProfile)
                .WithMany(sp => sp.Marks)
                .HasForeignKey(m => m.StudentProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Marks>()
                .HasOne(m => m.Subject)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Marks>()
                .HasOne(m => m.Semester)
                .WithMany()
                .HasForeignKey(m => m.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ObtainedMarks decimal precision (2 decimal places, max 999.99)
            modelBuilder.Entity<Marks>()
                .Property(m => m.ObtainedMarks)
                .HasPrecision(5, 2);

            // Subject relationships
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Semester)
                .WithMany(sem => sem.Subjects)
                .HasForeignKey(s => s.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Branch)
                .WithMany(b => b.Subjects)
                .HasForeignKey(s => s.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notice relationships
            modelBuilder.Entity<Notice>()
                .HasOne(n => n.CreatedByUser)
                .WithMany(u => u.NoticesCreated)
                .HasForeignKey(n => n.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
