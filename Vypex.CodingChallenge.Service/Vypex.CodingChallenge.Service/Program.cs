
using Scalar.AspNetCore;
using Vypex.CodingChallenge.API;
using Vypex.CodingChallenge.Domain;
using Vypex.CodingChallenge.Infrastructure;

namespace Vypex.CodingChallenge.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddApiModule()
                .AddDomainModule()
                .AddInfrastructureModule(builder.Configuration.GetConnectionString("DefaultConnection") ??
                    throw new ArgumentException("Connection string not specified"));

            builder.Services
                .AddControllers()
                .AddApiControllers();

            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
                app.UseCors(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/", () => "Vypex Coding Challenge service is running.");

            app.Run();
        }
    }
}
