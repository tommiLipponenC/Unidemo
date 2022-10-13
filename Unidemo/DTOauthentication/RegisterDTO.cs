using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Unidemo.DTOauthentication
{
    public class RegisterDTO
    {
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

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }
}
