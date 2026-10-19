using System.ComponentModel.DataAnnotations;

namespace HRMS.DTOs.UsersDTOs
{
    public class UpdateUserRequest
    {
        [Required]
        [MaxLength(100)] 
        public string FirstName { get; set; } = string.Empty;

        [Required] 
        [MaxLength(100)] 
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30)] 
        public string? PhoneNumber { get; set; }

        [MaxLength(300)] 
        public string? Address { get; set; }
    }
}
