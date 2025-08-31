using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SubMate.Core.Dtos.Auth;
using SubMate.Core.Dtos.User;
using SubMate.Core.Interfaces.Repositories;
using SubMate.Infrastructure.Data;
using SubMate.Infrastructure.Entities;
using System.Data;

namespace SubMate.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataEntities _dbContext;

        public UserRepository(DataEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserResponse> CreateUserAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var addUser = new User
            {
                EmailAddress = request.EmailAddress,
                Password = request.Password,
            };

            var newUser = await _dbContext.Users.AddAsync(addUser, ct);
            await _dbContext.SaveChangesAsync();
            if (newUser == null)
            {
                return null;
            }

            return new UserResponse
            {
                EmailAddress = addUser.Email,
                FirstName = addUser.FirstName,
                LastName = addUser.LastName,
            };
        }

        public async Task<UserResponse> GetUserByEmailAddressAsync(string email, CancellationToken ct = default)
        {
            var userExists = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
            if (userExists == null)
            {
                return null;
            }
            return new UserResponse
            {
                EmailAddress = userExists.Email,
                FirstName = userExists.FirstName,
                LastName = userExists.LastName,
            };
        }

        public async Task<UserResponse> GetUserByIdAsync(int id, CancellationToken ct = default)
        {
            var userExists = await _dbContext.Users.FindAsync(id);
            if (userExists == null)
            {
                return null;
            }

            return new UserResponse
            {
                EmailAddress = userExists.Email,
                FirstName = userExists.FirstName,
                LastName = userExists.LastName,
            };
        }

        public async Task<bool> CheckPasswordAsync(User user, string password, CancellationToken ct = default)
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);

            return result == PasswordVerificationResult.Success;
        }

        public async Task<IList<string>> GetUserRolesAsync(User user, CancellationToken ct = default)
        {
            var roles = await (from ur in _dbContext.UserRoles
                               join r in _dbContext.Roles on ur.RoleId equals r.Id
                               where ur.UserId == user.Id
                               select r.Name).ToListAsync(ct);

            return roles;
        }

        public async Task<IdentityResult> AddUserToRoleAsync(User user, string role, CancellationToken ct = default)
        {
            var roleEntity = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == role, ct);
            if (roleEntity == null)
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{role}' does not exist." });

            var exists = await _dbContext.UserRoles
                .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == roleEntity.Id, ct);

            if (!exists)
            {
                _dbContext.UserRoles.Add(new IdentityUserRole<int>
                {
                    UserId = user.Id,
                    RoleId = roleEntity.Id
                });

                await _dbContext.SaveChangesAsync(ct);
            }

            return IdentityResult.Success;
        }
    }
}


