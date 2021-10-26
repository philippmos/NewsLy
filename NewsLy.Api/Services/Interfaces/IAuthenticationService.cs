using Microsoft.AspNetCore.Identity;

namespace NewsLy.Api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        string GenerateJwtToken(IdentityUser identityUser);
    }
}