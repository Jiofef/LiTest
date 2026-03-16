using LiTest.Server.Core.Services;
using LiTest.Server.Services;
using LiTest.Server.Services.Community;
using LiTest.Shared.Core.Community;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace LiTest.Server.Core.Controllers
{
    [EnableRateLimiting("fixed")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserSecurityService _userSecurityService;
        private readonly IUserMainService _userMainService;
        public AuthController(IUserSecurityService uss, IUserMainService ums) 
        {
            _userSecurityService = uss;
            _userMainService = ums;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserAuthenticationDto dto)
        {
            var result = await _userSecurityService.LoginAsync(dto);

            if (!result.IsSuccess)
            {
                return result.ErrorCodes.Contains(StandardResultErrors.ACCESS_DENIED)
                    ? Unauthorized()
                    : BadRequest(new {errors = result.ErrorCodes, messages = result.ErrorMessages});
            }

            return Ok(result.Value);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto dto)
        {
            var result = await _userMainService.RegisterAndLoginAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(new { errors = result.ErrorCodes, messages = result.ErrorMessages });

            return Ok(result.Value);
        }
    }
}
