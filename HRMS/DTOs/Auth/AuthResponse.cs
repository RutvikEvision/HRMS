namespace HRMS.DTOs.Auth
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public DateTime ExpiresAtUtc { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

    }
}
