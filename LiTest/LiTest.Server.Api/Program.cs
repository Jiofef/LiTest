using LiTest.Server.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;

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

        app.Run();
    }

    private static void ConfigureDiBuilder(WebApplicationBuilder builder)
    {
        builder.Services.AddPooledDbContextFactory<LiTestDbContext>((options) =>
        {
            var connString = builder.Configuration.GetConnectionString("LiTestDefaultConnection");
            options.UseNpgsql(connString);
        });
    }
}