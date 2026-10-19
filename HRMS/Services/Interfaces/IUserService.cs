using HRMS.Common;
using HRMS.DTOs;
using HRMS.DTOs.UsersDTOs;

namespace HRMS.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> CreateAsync(CurrentUser me, CreateUserRequest request);
        Task<List<UserResponse>> GetAllAsync(CurrentUser me);
        Task<UserResponse> UpdateAsync(CurrentUser me, Guid userId, UpdateUserRequest request);
        Task DeleteAsync(CurrentUser me, Guid userId);
    }
}
