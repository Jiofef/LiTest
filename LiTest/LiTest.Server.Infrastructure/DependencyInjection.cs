using LiTest.Server.Core.Contracts.Data;
using LiTest.Server.Core.Security;
using LiTest.Server.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LiTest.Server.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ILiTestRepository, LiTestRepository>();
            services.AddSingleton<IJwtProvider, IJwtProvider>();

            return services;
        }
    }
}
