using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Unidemo.Data;

namespace Unidemo.Models
{
    public class StudentCourse
    {
        
        public string Id { get; set; }

        [ForeignKey("Id")]
        public ApplicationUser? ApplicationUser { get; set; }
        public Guid CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }
    }
}
