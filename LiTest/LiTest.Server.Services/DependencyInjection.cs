using FluentValidation;
using LiTest.Server.Core.Services;
using LiTest.Server.Services.Community;
using LiTest.Server.Services.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace LiTest.Server.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Adding validators
            services.AddValidatorsFromAssemblyContaining<UserRegistrationValidator>();

            // Services
            services.AddScoped<IUserMainService, UserMainService>();
            services.AddScoped<IUserSecurityService, UserSecurityService>();

            return services;
        }
    }
}
