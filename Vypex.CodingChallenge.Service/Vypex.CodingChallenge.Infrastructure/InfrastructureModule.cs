using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Vypex.CodingChallenge.Infrastructure.Data;

namespace Vypex.CodingChallenge.Infrastructure
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructureModule(this IServiceCollection services,
            string connectionString)
        {
            services.AddDbContext<CodingChallengeContext>(options => options
                .UseSqlite(connectionString));

            return services;
        }
    }
}
