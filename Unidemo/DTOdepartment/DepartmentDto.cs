using System.ComponentModel.DataAnnotations;

namespace Unidemo.DTOdepartment
{
    public class DepartmentDto
    {
        public Guid DepartmentId { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 25 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string DepartmentName { get; set; }
    }
}
