using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NewsLy.Api.Controllers
{
        public class HealthzController : ApiBaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Get()
        {
            return Ok("Healthy");
        }
    }
}
