using Microsoft.AspNetCore.Identity;

namespace SubMate.Infrastructure.Entities
{
    public class User : IdentityUser<int>
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
