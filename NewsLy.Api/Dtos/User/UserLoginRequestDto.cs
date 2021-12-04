using System.ComponentModel.DataAnnotations;

namespace NewsLy.Api.Dtos.User
{
    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}