using System.ComponentModel.DataAnnotations;

namespace SubMate.Core.Dtos.Auth
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email Address is required.")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Password is required.")] //Check the passowrd length here as well.
        public string Password { get; set; }
        [Required(ErrorMessage = "Firstname is required.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required.")]
        public string LastName { get; set; }
    }
}
