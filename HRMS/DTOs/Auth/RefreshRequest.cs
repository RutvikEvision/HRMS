using System.ComponentModel.DataAnnotations;

namespace HRMS.DTOs.Auth
{
    public class RefreshRequest
    {
        [Required] 
        public string RefreshToken { get; set; } = string.Empty;
    }
}
