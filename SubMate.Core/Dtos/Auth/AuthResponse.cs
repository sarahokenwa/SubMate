namespace SubMate.Core.Dtos.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public int? UserId { get; set; }
        public string EmailAddress { get; set; }
    }
}
