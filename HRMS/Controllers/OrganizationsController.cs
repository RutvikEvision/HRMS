using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.DTOs;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRMS.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public OrganizationsController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganizationRequest request)
        {
            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var organization = await _organizationService.CreateAsync(
                userId,
                request.Name,
                request.Address);


            return StatusCode(StatusCodes.Status201Created, new
            {
                organization.Id,
                organization.Name,
                organization.Address,
                Role = "Admin"
            });
        }

        [HttpGet("mine-organizations")]
        public async Task<IActionResult> GetMineOrganizations()
        {
            var userIdValue = User.FindFirstValue("sub");

            if(!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var memberships = await _organizationService.GetMineOrganizationsAsync(userId);

            var result = memberships.Select(m => new
            {
                m.Organization.Id,
                m.Organization.Name,
                m.Organization.Address,
                Role = m.Role.ToString()
            });

            return Ok(result);
        }
    }
}
