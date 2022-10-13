using System;
using System.Collections.Generic;
using System.Text;

namespace Unidemo.DTOauthentication
{
    public class RegisterResponseDTO
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
