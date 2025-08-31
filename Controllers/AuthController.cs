using Microsoft.AspNetCore.Mvc;
using SubMate.Core.Dtos.Auth;
using SubMate.Core.Interfaces.Services;

namespace SubMate.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {

            var response = await _authService.RegisterAsync(request, ct);
            return Ok(response);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {

            var response = await _authService.LoginAsync(request, ct);
            return Ok(response);
        }
    }
}

