﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Dtos;
using NewsLy.Api.Services.Interfaces;
using NewsLy.Api.Settings;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<TrackingController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(
            ILogger<TrackingController> logger,
            UserManager<IdentityUser> userManager,
            IAuthenticationService authenticationService
        )
        {
            _logger = logger;
            _userManager = userManager;
            _authenticationService = authenticationService;
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto userRegistrationRequestDto)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(userRegistrationRequestDto.Email);

                if (existingUser != null)
                {
                    return BadRequest(
                        new UserRegistrationResponseDto() {
                            Errors = new List<string>() {
                                "Email is already in use."
                            },
                            Success = false
                        }
                    );
                }

                var newUser = new IdentityUser() {
                    Email = userRegistrationRequestDto.Email,
                    UserName = userRegistrationRequestDto.UserName
                };

                IdentityResult userCreationIdentityResult = await _userManager.CreateAsync(
                    newUser,
                    userRegistrationRequestDto.Password
                );

                if (userCreationIdentityResult.Succeeded)
                {
                    return Ok(
                        new UserRegistrationResponseDto() {
                            Token = _authenticationService.GenerateJwtToken(newUser),
                            Success = true
                        }
                    );
                }
                else
                {
                    return BadRequest(
                        new UserRegistrationResponseDto() {
                            Errors = userCreationIdentityResult.Errors.Select(
                                x => x.Description
                            ).ToList(),
                            Success = false
                        }
                    );
                }
            }
       
            return BadRequest(
                new UserRegistrationResponseDto() {
                    Errors = new List<string>() {
                        "Invalid Model"
                    },
                    Success = false
                }
            );
        }
    }
}