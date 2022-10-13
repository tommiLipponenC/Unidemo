using System;
using System.Collections.Generic;
using System.Text;

namespace Unidemo.DTOauthentication
{
    public class LoginResponseDTO
    {
        public string AccessToken { get; set; } 
        public DateTime? ExpiryDate { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }       
    }
}
