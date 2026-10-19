using FluentValidation;
using HRMS.Common;
using HRMS.DTOs.UsersDTOs;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,HR")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<CreateUserRequest> _createValidator;

        public UsersController(IUserService userService, IValidator<CreateUserRequest> createValidator)
        {
            _userService = userService;
            _createValidator = createValidator;
        }
        private CurrentUser Me => new(
            Guid.Parse(User.FindFirstValue("sub")!),
            Guid.Parse(User.FindFirstValue("organization_id")!),
            User.FindFirstValue("role")!);


        [HttpGet]
        public async Task<ActionResult<List<UserResponse>>> GetAll()
        {
            var response = await _userService.GetAllAsync(Me);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var response = await _userService.CreateAsync(Me, request);

            return StatusCode(201, response);
        }


        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserResponse>> Update(Guid id, UpdateUserRequest request)
        {
            var response = await _userService.UpdateAsync(Me, id, request);

            return Ok(response);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(Me, id);
            return NoContent();
        }
    }
}
