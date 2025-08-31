using Microsoft.AspNetCore.Identity;
using SubMate.Core.Dtos.Auth;
using SubMate.Core.Dtos.User;
using SubMate.Core.Enums;
using SubMate.Core.Exceptions;
using SubMate.Core.Interfaces.Repositories;
using SubMate.Core.Interfaces.Services;
using SubMate.Infrastructure.Entities;

namespace SubMate.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository,
                          IPasswordHasher<User> passwordHasher) 
        { 
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResponse> RegisterUserAsync(RegisterRequest request, CancellationToken ct = default)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByEmailAddressAsync(request.EmailAddress, ct);
                if (existingUser != null)
                    throw new BaseException(ExceptionType.ALREADY_EXIST, "User with this email address already exists.");

                var user = new User
                {
                    EmailAddress = request.EmailAddress,
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

               var newUser = await _userRepository.CreateUserAsync(request, ct);

               return newUser;
            }
            catch(BaseException e)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BaseException(ExceptionType.OPERATION_FAILED, "Failed to create user, Please contact your administrator.");
            }
            
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _userRepository.GetUserByEmailAsync(email, ct);
        }
    }
}
