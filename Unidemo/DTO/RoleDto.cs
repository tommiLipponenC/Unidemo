using System.ComponentModel.DataAnnotations;

namespace Unidemo.DTO
{
    public class RoleDto
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 25 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string Name { get; set; }
    }
}
