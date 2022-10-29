using System.ComponentModel.DataAnnotations;
using Unidemo.DTOcourse;
using Unidemo.Models;

namespace Unidemo.DTOstudentcourse
{
    public class StudentDetailDto
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public ICollection<CourseDto>? Coursesss { get; set; }
       
    }
}
