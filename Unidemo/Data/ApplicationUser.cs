using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Unidemo.Models;

namespace Unidemo.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 25 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string? Firstname { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Max lenght is 25 characters")]
        [MinLength(2, ErrorMessage = "Min lenght is 2 characters")]
        public string? Lastname { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
