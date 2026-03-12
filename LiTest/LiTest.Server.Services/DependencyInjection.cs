using FluentValidation;
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



            return services;
        }
    }
}
