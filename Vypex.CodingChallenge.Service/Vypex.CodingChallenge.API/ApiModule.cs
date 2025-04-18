using Microsoft.Extensions.DependencyInjection;
using Vypex.CodingChallenge.API.Services;

namespace Vypex.CodingChallenge.API
{
    public static class ApiModule
    {
        public static IServiceCollection AddApiModule(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeLeaveService, EmployeeLeaveService>();
            return services;
        }

        public static IMvcBuilder AddApiControllers(this IMvcBuilder builder)
        {
            return builder.AddApplicationPart(typeof(ApiModule).Assembly);
        }
    }
}
