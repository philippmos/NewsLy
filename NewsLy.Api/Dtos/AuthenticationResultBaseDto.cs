using System.Collections.Generic;

namespace NewsLy.Api.Dtos
{
    public class AuthenticationResultBaseDto
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}