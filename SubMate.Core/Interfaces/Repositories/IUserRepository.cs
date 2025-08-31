using Microsoft.AspNetCore.Identity;
using SubMate.Core.Dtos.Auth;
using SubMate.Core.Dtos.User;

namespace SubMate.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserResponse> CreateUserAsync(RegisterRequest request, CancellationToken ct = default);
        Task<UserResponse?> GetUserByEmailAddressAsync(string email, CancellationToken ct = default);
        Task<UserResponse?> GetUserByIdAsync(int id, CancellationToken ct = default);
        Task<bool> CheckPasswordAsync(Dtos.User user, string password, CancellationToken ct = default);
        Task<IList<string>> GetUserRolesAsync(User user, CancellationToken ct = default);
        Task<IdentityResult> AddUserToRoleAsync(User user, string role, CancellationToken ct = default)    }

}
