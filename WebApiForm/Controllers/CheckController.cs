using Microsoft.AspNetCore.Mvc;

namespace WebApiForm.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckController : ControllerBase
    {
        [HttpGet]
        public IActionResult Check()
        {
            return Ok("OK");
        }
    }
}