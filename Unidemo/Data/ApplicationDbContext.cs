using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Unidemo.Models;

namespace Unidemo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.AppUsers)
                .WithMany(c => c.Courses)
                .UsingEntity<StudentCourse>(
                    c => c
                        .HasOne(uc => uc.ApplicationUser)
                        .WithMany(s => s.StudentCourses)
                        .HasForeignKey(uc => uc.Id),
                    c => c
                        .HasOne(uc => uc.Course)
                        .WithMany(c => c.StudentCourses)
                        .HasForeignKey(uc => uc.CourseId),
                    c =>
                    {
                        c.HasKey(s => new {s.Id, s.CourseId});
                    });

        }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set;}
        public DbSet<Grade> Grades { get; set; }
    }
}