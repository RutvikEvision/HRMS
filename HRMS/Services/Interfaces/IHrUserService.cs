namespace HRMS.Services.Interfaces
{
    public interface IHrUserService
    {
        Task<(bool Success, string Message, Guid? UserId)> CreateAsync(
            Guid adminUserId,
            Guid organizationId,
            string email,
            string password
        );
    }
}
