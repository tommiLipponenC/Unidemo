using System.ComponentModel.DataAnnotations;

namespace Unidemo.DTOgrade
{
    public class CreateGradeDto
    {
        public string? TeacherName { get; set; }
        public string? StudentName { get; set; }

        [Range(1, 10)]
        public int? GradeValue { get; set; }
     //   public DateTime? LastModified { get; set; }
        public Guid? CourseId { get; set; }
    }
}
