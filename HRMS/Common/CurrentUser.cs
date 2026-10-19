namespace HRMS.Common
{
    public record CurrentUser(Guid UserId, Guid OrganizationId, string Role);
}
