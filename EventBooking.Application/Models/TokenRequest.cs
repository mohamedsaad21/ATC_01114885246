using System.ComponentModel.DataAnnotations;

namespace EventBooking.Application.Models
{
    public class TokenRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
