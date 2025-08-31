
using Microsoft.AspNetCore.Identity;

namespace SubMate.Core.Dtos.User
{
   // public class UserResponse : IdentityUser<int>
    public class UserResponse 
    {
        public string EmailAddress { get; set; } 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
