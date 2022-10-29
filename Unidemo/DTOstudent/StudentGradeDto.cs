using System.ComponentModel.DataAnnotations;
using Unidemo.DTOcourse;

namespace Unidemo.DTOstudent
{
    public class StudentGradeDto
    {
        public string? StudentName { get; set; }
        public string? TeacherName { get; set; }
        public string? CourseName { get; set; }
        public int? GradeValue { get; set; }
        public ICollection<CourseDto>? Coursesss { get; set; }
    }
}
