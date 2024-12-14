using AuthorTools.Api.Options;
using AuthorTools.Api.Routes;
using AuthorTools.Api.Services;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Data.Repositories;
using AuthorTools.Data.Repositories.Interfaces;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;

namespace AuthorTools.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // KeyVault setup
        if (builder.Environment.IsProduction())
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{builder.Configuration.GetSection("KeyVaultName").Value
                    ?? throw new Exception($"Failed to read appsetting {JsonSerializer.Serialize(builder.Configuration)}")}.vault.azure.net/"),
                new DefaultAzureCredential());
        }

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

        // JWT Authentication
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new ArgumentException("Error getting JWT Settings");
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.Authority = jwtSettings.Issuer;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        builder.Services.AddAuthorization();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));

        var environment = builder.Configuration.GetValue<string>("Application:Environment")
            ?? throw new ArgumentException("Error getting Application:Environment");

        // Repos
        var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new ArgumentException("Error getting MongoDbSettings");

        builder.Services.AddSingleton<ICharacterRepository, CharacterRepository>(sp =>
            new CharacterRepository(mongoDbSettings.DatabaseName, mongoDbSettings.ConnectionString, environment));

        builder.Services.AddSingleton<IUserSettingRepository, UserSettingRepository>(sp =>
            new UserSettingRepository(mongoDbSettings.DatabaseName, mongoDbSettings.ConnectionString, environment));

        // Services
        builder.Services.AddScoped<IIdentityProvider, UserProvider>();
        builder.Services.AddScoped<AzureBlobService>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<ICharacterService, CharacterService>();
        builder.Services.AddScoped<IUserSettingService, UserSettingService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRoutes();

        app.Run();
    }
}
