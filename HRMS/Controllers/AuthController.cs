using HRMS.Data.Entities;
using HRMS.DTOs.Auth;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);

            return StatusCode(201, response);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request)
        {
            var response = await _authService.RefreshAsync(request);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(Guid.Parse(User.FindFirstValue("sub")!));
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = User.FindFirstValue("sub"),
                Email = User.FindFirstValue("email"),
                Name = User.FindFirstValue("name"),
                OrganizationId = User.FindFirstValue("organization_id"),
                Role = User.FindFirstValue("role")
            });
        }
    }
}
