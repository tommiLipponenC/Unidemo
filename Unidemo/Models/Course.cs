using System.ComponentModel.DataAnnotations;
using Unidemo.Data;

namespace Unidemo.Models
{
    public class Course
    {
        [Key]
        public Guid CourseId { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 25 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string CourseName { get; set; }
        public int Points { get; set; }
        public ICollection<ApplicationUser>? AppUsers { get; set; }
        public List<StudentCourse>? StudentCourses { get; set; }
        public List<Grade>? Studentsgrades { get; set; }
    }
}
