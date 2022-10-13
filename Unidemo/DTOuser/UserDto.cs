using System.ComponentModel.DataAnnotations;

namespace Unidemo.DTOuser
{
    public class UserDto
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 50 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 50 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string Lastname { get; set; }

        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
