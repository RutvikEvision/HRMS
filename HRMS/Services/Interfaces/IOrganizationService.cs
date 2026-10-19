using HRMS.Data.Entities;
using HRMS.DTOs.Auth;
using HRMS.DTOs.OrganizationDTOs;

namespace HRMS.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<AuthResponse> CreateWithAdminAsync(Guid userId, CreateOrganizationRequest request);
        Task<List<MineOrganizationResponse>> GetMineOrganizationsAsync(Guid userId);
        Task<AuthResponse> SelectAsync(Guid userId, Guid organizationId);

    }
}
