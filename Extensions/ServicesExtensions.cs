using SubMate.Core.Interfaces.Repositories;
using SubMate.Core.Interfaces.Services;
using SubMate.Infrastructure.Repositories;
using SubMate.Infrastructure.Services;

namespace SubMate.Api.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddAppServices(this IServiceCollection services,
           IConfiguration configuration)
        {
            //Services
            //services.AddScoped<IScheduleManager, ScheduleManager>();
            



            //Repositories
            #region Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            

            #endregion

            //Third party service
            #region service
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
           


            #endregion service

            //Configurations
            //services.AddSingleton(configuration.GetSection("ApiOptions").Get<ApiOptions>());
            

        }
    }
}
