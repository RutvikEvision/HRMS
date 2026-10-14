using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers
{
    [Route("api/organizations/{organizationId:guid}/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizationUsers(Guid organizationId)
        {
            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized();
            }

            var result = await _userService.GetOrganizationUsersAsync(currentUserId, organizationId);

            if (!result.IsAuthorized)
            {
                return Forbid();
            }

            return Ok(result.Response);
        }
    }
}
