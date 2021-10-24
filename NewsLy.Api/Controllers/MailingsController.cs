using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsLy.Api.Models;
using NewsLy.Api.Repositories.Interfaces;
using NewsLy.Api.Services;

namespace NewsLy.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MailingsController : ControllerBase
    {

        private readonly ILogger<MailingsController> _logger;
        private readonly IMailingService _mailingService;
        private readonly IContactRequestRepository _contactRequestRepository;

        public MailingsController(
            ILogger<MailingsController> logger,
            IMailingService mailingService,
            IContactRequestRepository contactRequestRepository
        )
        {
            _logger = logger;
            _mailingService = mailingService;
            _contactRequestRepository = contactRequestRepository;
        }

        [HttpGet]
        public List<ContactRequest> Get()
        {
            return _contactRequestRepository.GetAll();
        }

        [HttpPost]
        public async Task<IActionResult> SendMailAsync([FromForm]ContactRequest mailRequest)
        {
            if (string.IsNullOrEmpty(mailRequest.ToEmail) && !mailRequest.ToMailingListId.HasValue )
            {
                return BadRequest("ToEmail or ToMailingListId Parameter is required.");
            }

            try
            {
                await _mailingService.SendMailingAsync(mailRequest);
                
                _contactRequestRepository.Add(mailRequest);

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
