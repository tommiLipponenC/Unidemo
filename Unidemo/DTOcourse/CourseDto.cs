using System.ComponentModel.DataAnnotations;

namespace Unidemo.DTOcourse
{
    public class CourseDto
    {
        public Guid CourseId { get; set; }
        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 25 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
      
        public string CourseName { get; set; }
        [Required]
        [Range(1,10)]
        public int Points { get; set; }
    }
}
