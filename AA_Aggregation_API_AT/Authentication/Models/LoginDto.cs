using System.ComponentModel.DataAnnotations;

namespace AA_Aggregation_API_AT.Authentication.Models
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
