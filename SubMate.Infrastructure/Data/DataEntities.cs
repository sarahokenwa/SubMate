using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubMate.Infrastructure.Entities;

namespace SubMate.Infrastructure.Data
{
    public class DataEntities : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DataEntities(DbContextOptions<DataEntities> options) : base(options)
        {
            
        }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
