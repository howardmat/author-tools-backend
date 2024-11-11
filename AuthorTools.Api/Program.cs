
using AuthorTools.Api.Options;
using AuthorTools.Api.Repositories;
using AuthorTools.Api.Services;

namespace AuthorTools.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // CORS
        var corsOptions = builder.Configuration.GetSection("Cors").Get<CorsOptions>();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.WithOrigins(corsOptions?.AcceptedOrigins ?? [])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        // Add services to the container.
        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));

        // Repo
        builder.Services.Configure<CharacterDbOptions>(builder.Configuration.GetSection("CharacterDbSettings"));
        builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>();

        // Services
        builder.Services.AddSingleton<FileStorageService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthorization();

        app.UseRoutes();

        app.Run();
    }
}
