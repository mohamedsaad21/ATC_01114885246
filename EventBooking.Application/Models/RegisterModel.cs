using System.ComponentModel.DataAnnotations;

namespace EventBooking.Application.Models
{
    public class RegisterModel
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        [Required, MaxLength(100)]
        public string Username { get; set; }
        [Required, EmailAddress, MaxLength(128)]
        public string Email { get; set; }
        [Required, DataType(DataType.Password), MaxLength(256)]
        public string Password { get; set; }
    }
}
