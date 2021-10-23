using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Models;
using NewsLy.Api.Services;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailingsController : ControllerBase
    {

        private readonly ILogger<MailingsController> _logger;
        private readonly IMailingService _mailingService;

        public MailingsController(
            ILogger<MailingsController> logger,
            IMailingService mailingService)
        {
            _logger = logger;
            _mailingService = mailingService;
        }

        [HttpGet]
        public KeyValuePair<string, string> Get()
        {
            return new KeyValuePair<string, string> ( "status", "success" );
        }

        [HttpPost("contact")]
        public async Task<IActionResult> SendMailAsync([FromForm]ContactRequest mailRequest)
        {
            try
            {
                await _mailingService.SendMailAsync(mailRequest);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            
            return StatusCode(500);
        }
    }
}
