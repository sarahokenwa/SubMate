

using SubMate.Core.Dtos.Auth;
using SubMate.Core.Dtos.User;

namespace SubMate.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponse> RegisterUserAsync(RegisterRequest request, CancellationToken ct = default);
    }
}
