using FluentValidation;
using HRMS.DTOs;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRMS.Controllers
{
    [Route("api/organizations/{organizationId:guid}/employees")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IValidator<CreateEmployeeRequest> _createValidator;
        private readonly IValidator<UpdateEmployeeRequest> _updateValidator;

        public EmployeesController(
            IEmployeeService employeeService, 
            IValidator<CreateEmployeeRequest> createValidator,
            IValidator<UpdateEmployeeRequest> updateValidator)
        {
            _employeeService = employeeService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid organizationId, CreateEmployeeRequest request)
        {
            var validationResult = await _createValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        message = error.ErrorMessage
                    })
                });
            }

            var userIdValue = User.FindFirstValue("sub");

            if (!Guid.TryParse(userIdValue, out var hrUserId))
            {
                return Unauthorized();
            }

            var result = await _employeeService.CreateAsync(hrUserId, organizationId, request);

            if (!result.IsAuthorized)
            {
                return Forbid();
            }

            var employee = result.Employee!;

            return StatusCode(StatusCodes.Status201Created, new
            {
                employee.Id,
                employee.OrganizationId,
                employee.FirstName,
                employee.LastName,
                employee.Email,
                employee.PhoneNumber,
                employee.Address,
                employee.CreatedAtUtc
            });
        }

        [HttpPut("{employeeId:guid}")]
        public async Task<IActionResult> Update(Guid organizationId, Guid employeeId, UpdateEmployeeRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        message = error.ErrorMessage
                    })
                });
            }

            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var hrUserId))
            {
                return Unauthorized();
            }

            var result = await _employeeService.UpdateAsync(hrUserId, organizationId, employeeId, request);

            if (!result.IsAuthorized)
            {
                return Forbid();
            }

            if (!result.Found)
            {
                return NotFound(new
                {
                    message = "Employee not found"
                });
            }

            return NoContent();
        }

        [HttpDelete("{employeeId:guid}")]
        public async Task<IActionResult> Delete(Guid organizationId, Guid employeeId)
        {
            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var hrUserId))
            {
                return Unauthorized();
            }

            var result = await _employeeService.DeleteAsync(organizationId, hrUserId, employeeId);

            if (!result.IsAuthorized)
            {
                return Forbid();
            }

            if (!result.Found)
            {
                return NotFound(new
                {
                    message = "Employee not found"
                });
            }

            return NoContent();
        }


        [HttpGet("{employeeId}/details")]
        public async Task<IActionResult> GetDetails(Guid organizationId, Guid employeeId)
        {
            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var currentUserId))
            {
                return Unauthorized();
            }

            var result = await _employeeService.GetDetailsAsync(currentUserId, organizationId, employeeId);

            if (!result.IsAuthorized)
            {
                return Forbid();
            }

            if (result.Details is null)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return Ok(result.Details);
        }
    }
}
