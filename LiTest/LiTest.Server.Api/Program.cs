using LiTest.Server.Infrastructure;
using LiTest.Server.Infrastructure.Data;
using LiTest.Server.Infrastructure.Security;
using LiTest.Server.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Threading.RateLimiting;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddSwaggerGen();

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        // Anti DDoS
        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", opt =>
            {
                opt.Window = TimeSpan.FromSeconds(1);
                opt.PermitLimit = 5;                
                opt.QueueLimit = 0;                 
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.Limits.MaxRequestBodySize = 10 * 1024;

            serverOptions.Limits.MinRequestBodyDataRate =
                new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
        });

        ConfigureDiBuilder(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.UseRateLimiter();   

        app.Run();
    }

    private static void ConfigureDiBuilder(WebApplicationBuilder builder)
    {
        // Services
        builder.Services.AddInfrastructureServices();
        builder.Services.AddApplicationServices();
        
        // Database connection
        builder.Services.AddPooledDbContextFactory<LiTestDbContext>((options) =>
        {
            var connString = builder.Configuration.GetConnectionString("LiTestDefaultConnection");
            options.UseNpgsql(connString);
        });

        // Logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Host.UseSerilog();

        // Options
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtSettings"));
    }
}