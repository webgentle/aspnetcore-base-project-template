using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Web.App.Managers;
using Web.App.Models.Account;
using Web.App.Services;

namespace Web.App.Helpers
{
    public static class DependecyResolver
    {
        public static void Resolve(IServiceCollection services)
        {
            services.AddTransient<IAccountManager, AccountManager>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppUserClaimsPrincipalFactory>();
        }
    }
}
