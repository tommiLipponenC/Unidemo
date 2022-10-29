using System.ComponentModel.DataAnnotations;

namespace Unidemo.Models
{
    public class Grade
    {
        public Guid GradeId { get; set; }
        public string? TeacherName { get; set;}
        public string? StudentName { get; set; }

        [Range(1, 10)]
        public int? GradeValue { get; set;}
        public DateTime? LastModified { get; set; } = DateTime.UtcNow;
        public Guid? CourseId { get; set; }
        public Course? Course { get; set; }

    }
}
