using System.ComponentModel.DataAnnotations;

namespace NewsLy.Api.Dtos
{
    public class UserRegistrationRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}