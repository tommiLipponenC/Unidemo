using Unidemo.Data;

namespace Unidemo.Models
{
    public class StudentCourse
    {
        public Guid Id { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
    }
}
