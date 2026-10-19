using HRMS.Data;
using HRMS.Data.Entities;
using HRMS.DTOs.Auth;
using HRMS.DTOs.OrganizationDTOs;
using HRMS.Services;
using HRMS.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private Guid UserId => Guid.Parse(User.FindFirstValue("sub")!);

        [HttpPost]
        public async Task<ActionResult<AuthResponse>> Create(CreateOrganizationRequest request)
        {
            var response = await _organizationService.CreateWithAdminAsync(UserId, request);

            return StatusCode(201, response);
        }

        [HttpGet("mine-organizations")]
        public async Task<ActionResult<List<MineOrganizationResponse>>> GetMineOrganizations()
        {
            var response = await _organizationService.GetMineOrganizationsAsync(UserId);

            return Ok(response);
        }

        [HttpPost("{id:guid}/select")]
        public async Task<ActionResult<AuthResponse>> Select(Guid id)
        {
            var response = await _organizationService.SelectAsync(UserId, id);

            return Ok(response);
        }
    }
}
