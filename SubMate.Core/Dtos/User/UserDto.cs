namespace SubMate.Core.Dtos.User
{
    public class UserDto
    {
        public int? Id { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
