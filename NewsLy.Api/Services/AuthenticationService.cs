using System;
using System.Text;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Settings;
using NewsLy.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace NewsLy.Api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;

        public AuthenticationService(
            ILogger<AuthenticationService> logger,
            IOptions<JwtSettings> jwtSettings,
            IConfiguration configuration
        )
        {
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
        }


        public string GenerateJwtToken(IdentityUser identityUser)
        {
            int.TryParse(_configuration["JwtTokenExpirationTimeInSeconds"], out var tokenExpirationTimeInSeconds);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new System.Security.Claims.ClaimsIdentity(
                    new [] {
                        new Claim("Id", identityUser.Id),
                        new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, identityUser.Email),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }
                ),
                Expires = DateTime.UtcNow.AddSeconds(tokenExpirationTimeInSeconds),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var jwtTokenHandler = new JwtSecurityTokenHandler();

            return jwtTokenHandler.WriteToken(
                jwtTokenHandler.CreateToken(tokenDescriptor)
            );
        }      
    }
}
