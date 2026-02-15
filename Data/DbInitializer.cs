using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Ensure DB created
            await context.Database.EnsureCreatedAsync();

            // Seed Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("Student"))
                await roleManager.CreateAsync(new IdentityRole("Student"));

            // Seed admin user
            var adminEmail = "admin@studentportal.local";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, FullName = "Administrator" };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
            else
            {
                // Ensure admin has the Admin role
                if (!await userManager.IsInRoleAsync(admin, "Admin"))
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed old courses (legacy)
            if (!context.Courses.Any())
            {
                context.Courses.AddRange(
                    new Course { Name = "Mathematics" },
                    new Course { Name = "Physics" },
                    new Course { Name = "Chemistry" }
                );
                await context.SaveChangesAsync();
            }

            // Seed Branches
            if (!context.Branches.Any())
            {
                context.Branches.AddRange(
                    new Branch { Name = "Electrical Engineering", Code = "EE", CreatedAt = DateTime.UtcNow },
                    new Branch { Name = "Mechanical Engineering", Code = "ME", CreatedAt = DateTime.UtcNow },
                    new Branch { Name = "Civil Engineering", Code = "CE", CreatedAt = DateTime.UtcNow },
                    new Branch { Name = "Computer Engineering", Code = "CSE", CreatedAt = DateTime.UtcNow },
                    new Branch { Name = "Automobile Engineering", Code = "AE", CreatedAt = DateTime.UtcNow },
                    new Branch { Name = "Electronics Engineering", Code = "EC", CreatedAt = DateTime.UtcNow }
                );
                await context.SaveChangesAsync();
            }

            // Seed Semesters
            if (!context.Semesters.Any())
            {
                var semesters = new List<Semester>();
                for (int i = 1; i <= 6; i++)
                {
                    semesters.Add(new Semester
                    {
                        SemesterNumber = i,
                        StartDate = DateTime.UtcNow.AddMonths(-6 + i * 3),
                        EndDate = DateTime.UtcNow.AddMonths(-3 + i * 3),
                        IsCommon = i <= 2,  // Semester 1 & 2 are common
                        CreatedAt = DateTime.UtcNow
                    });
                }
                context.Semesters.AddRange(semesters);
                await context.SaveChangesAsync();
            }

            // Seed Subjects
            if (!context.Subjects.Any())
            {
                var branches = await context.Branches.ToListAsync();
                var semesters = await context.Semesters.ToListAsync();

                var subjects = new List<Subject>();

                // Common subjects for Semester 1
                var sem1Subjects = new[] { "Mathematics I", "Physics I", "Chemistry I", "English" };
                foreach (var subj in sem1Subjects)
                {
                    subjects.Add(new Subject
                    {
                        Name = subj,
                        Code = $"CS{semesters[0].SemesterId}01",
                        Credits = 4,
                        MaxMarks = 100,
                        PassingMarks = 35,
                        SemesterId = semesters[0].SemesterId,
                        BranchId = null, // Common for all
                        CreatedAt = DateTime.UtcNow
                    });
                }

                // Common subjects for Semester 2
                var sem2Subjects = new[] { "Mathematics II", "Physics II", "Chemistry II", "Technical Communication" };
                var idx = 0;
                foreach (var subj in sem2Subjects)
                {
                    subjects.Add(new Subject
                    {
                        Name = subj,
                        Code = $"CS{semesters[1].SemesterId}0{idx + 1}",
                        Credits = 4,
                        MaxMarks = 100,
                        PassingMarks = 35,
                        SemesterId = semesters[1].SemesterId,
                        BranchId = null, // Common for all
                        CreatedAt = DateTime.UtcNow
                    });
                    idx++;
                }

                // Branch-specific subjects for Later Semesters (3-6)
                var branchSpecificSubjects = new Dictionary<int, string[]>
                {
                    { 0, new[] { "Circuit Theory", "Digital Electronics", "Electromagnetic Theory" } },  // EE
                    { 1, new[] { "Thermodynamics", "Mechanics", "Manufacturing Processes" } },          // ME
                    { 2, new[] { "Structural Analysis", "Construction Materials", "Surveying" } },     // CE
                    { 3, new[] { "Data Structures", "Algorithms", "Database Systems" } },              // CSE
                    { 4, new[] { "Automotive Systems", "Engine Design", "Vehicle Dynamics" } },        // AE
                    { 5, new[] { "Analog Electronics", "Digital Circuits", "Microprocessors" } }      // EC
                };

                for (int semIdx = 2; semIdx < 6; semIdx++)
                {
                    for (int branchIdx = 0; branchIdx < branches.Count; branchIdx++)
                    {
                        for (int subjIdx = 0; subjIdx < 3; subjIdx++)
                        {
                            subjects.Add(new Subject
                            {
                                Name = branchSpecificSubjects[branchIdx][subjIdx],
                                Code = $"{branches[branchIdx].Code}{semesters[semIdx].SemesterId}{subjIdx + 1:00}",
                                Credits = 4,
                                MaxMarks = 100,
                                PassingMarks = 35,
                                SemesterId = semesters[semIdx].SemesterId,
                                BranchId = branches[branchIdx].BranchId,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                }

                context.Subjects.AddRange(subjects);
                await context.SaveChangesAsync();
            }
        }
    }
}
