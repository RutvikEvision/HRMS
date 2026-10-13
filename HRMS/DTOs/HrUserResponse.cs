namespace HRMS.DTOs
{
    public class HrUserResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }
}
