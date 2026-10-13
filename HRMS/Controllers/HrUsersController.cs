using HRMS.DTOs;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers
{
    [Route("api/organizations/{organizationId:guid}/hr-users")]
    [ApiController]
    public class HrUsersController : ControllerBase
    {
        private readonly IHrUserService _hrUserService;

        public HrUsersController(IHrUserService hrUserService)
        {
            _hrUserService = hrUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid organizationId, CreateHrUserRequest request)
        {
            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var adminUserId))
            {
                return Unauthorized();
            }

            var result = await _hrUserService.CreateAsync(
                adminUserId,
                organizationId,
                request.Email,
                request.Password);

            if (!result.Success)
            {
                if (result.Message.Contains("already a member", StringComparison.OrdinalIgnoreCase))
                {
                    return Conflict(new { message = result.Message });
                }

                if (result.Message.Contains("not an active Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                return BadRequest(new { message = result.Message });
            }

            return StatusCode(StatusCodes.Status201Created, new
            {
                UserId = result.UserId,
                Email = request.Email.Trim(),
                Role = "HR",
                Message = result.Message
            });
        }
    }
}
