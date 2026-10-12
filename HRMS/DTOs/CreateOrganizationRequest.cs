using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace HRMS.DTOs
{
    public class CreateOrganizationRequest
    {
        [Required]
        [StringLength(150, MinimumLength = 2)]

        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(300, MinimumLength = 5)]
        public string Address { get; set; } = string.Empty;
    }
}
