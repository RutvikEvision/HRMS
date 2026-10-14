namespace HRMS.Services.Interfaces
{
    public interface IHrUserService
    {
        Task<(bool Success, string Message, Guid? UserId)> CreateAsync(Guid adminUserId,Guid organizationId,string email,string password);
        Task<(bool IsAuthorized, bool Found)> DeactivateAsync(Guid adminUserId, Guid organizationId, Guid hrUserId);

    }
}
