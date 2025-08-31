
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SubMate.Core.Dtos.Auth;
using SubMate.Core.Dtos.User;
using SubMate.Core.Enums;
using SubMate.Core.Exceptions;
using SubMate.Core.Interfaces.Repositories;
using SubMate.Core.Interfaces.Services;
using SubMate.Infrastructure.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SubMate.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _config;

        public AuthService(IUserRepository userRepo, RoleManager<IdentityRole<int>> roleManager, IConfiguration config)
        {
            _userRepo = userRepo;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            try
            {
                var existing = await _userRepo.GetUserByEmailAddressAsync(request.EmailAddress, ct);
                if (existing != null)
                    throw new BaseException(ExceptionType.ALREADY_EXIST, "User already exists with this email");

                var user = new User
                {
                    UserName = request.EmailAddress,
                    EmailAddress = request.EmailAddress
                };

                var createUser = await _userRepo.CreateUserAsync(request, ct);
                if (createUser == null)
                  throw new BaseException(ExceptionType.BAD_REQUEST, "Failed to create user, kindly contact your administrator.");

                var roleName = "User";
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }

                await _userRepo.AddUserToRoleAsync(user, roleName, ct);

                var roles = await _userRepo.GetUserRolesAsync(user, ct);
                var token = GenerateJwtToken(user, roles);

                return new AuthResponse
                {
                    Token = token.Token,
                    ExpiresAt = token.ExpiresAt,
                    UserId = user.Id,
                    EmailAddress = user.EmailAddress
                };
            }
            catch(BaseException e)
            {
                throw;
            }
            catch (Exception ex)
            { 
                throw new BaseException(ExceptionType.OPERATION_FAILED, "An error occured while registering user. Please contact your system administrator.")
            }
           
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userRepo.GetUserByEmailAddressAsync(request.EmailAddress, ct);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var ok = await _userRepo.CheckPasswordAsync(user, request.Password, ct);
            if (!ok)
                throw new UnauthorizedAccessException("Invalid credentials");

            var roles = await _userRepo.GetUserRolesAsync(user, ct);
            var token = GenerateJwtToken(user, roles);

            return new AuthResponse
            {
                Token = token.Token,
                ExpiresAt = token.ExpiresAt,
                UserId = user.Id,
                EmailAddress = user.EmailAddress
            };
        }

        private (string Token, DateTime ExpiresAt) GenerateJwtToken(User user, IList<string> roles)
        {
            var jwtSection = _config.GetSection("JwtSettings");
            var key = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var issuer = jwtSection["Issuer"] ?? "SubscriptionManagerApi";
            var audience = jwtSection["Audience"] ?? "SubscriptionClient";
            var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "60");

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            // add role claims
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            return (Token: tokenString, ExpiresAt: tokenDescriptor.Expires!.Value);
        }
    }
}
