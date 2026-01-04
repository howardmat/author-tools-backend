using AuthorTools.Api.Handlers;
using AuthorTools.Api.Options;
using AuthorTools.Api.Routes;
using AuthorTools.Api.Services;
using AuthorTools.Api.Services.Interfaces;
using AuthorTools.Common.Options;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories;
using AuthorTools.Data.Repositories.Interfaces;
using Azure.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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
            var keyVaultName = builder.Configuration.GetSection("KeyVaultName").Value
                ?? throw new Exception($"Failed to read appsetting {JsonSerializer.Serialize(builder.Configuration)}");

            builder.Configuration.AddAzureKeyVault(
                new Uri($"https://{keyVaultName}.vault.azure.net/"),
                new DefaultAzureCredential());
        }

        // Global Exception Handling
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // CORS
        var corsOptions = builder.Configuration.GetSection("Cors").Get<CorsOptions>();
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(corsOptions?.AcceptedOrigins ?? [])
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // JWT Authentication
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
            ?? throw new ArgumentException("Error getting JWT Settings");
        builder.Services.AddAuthentication()
            .AddJwtBearer(x =>
            {
                x.Authority = jwtSettings.Issuer;
                x.Audience = jwtSettings.Audience;
                x.TokenValidationParameters = new()
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
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                // Ensure instances exist
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                // Add Bearer security scheme 
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme."
                };

                // Apply security requirement globally
                document.Security = [
                    new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    }
                ];

                // Set the host document for all elements
                // including the security scheme references
                document.SetReferenceHostDocument();

                return Task.CompletedTask;
            });
        });

        // Appsettings to IOptions
        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection("Application"));

        var environment = builder.Configuration.GetValue<string>("Application:Environment")
            ?? throw new ArgumentException("Error getting Application:Environment");

        // Repos
        var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
            ?? throw new ArgumentException("Error getting MongoDbSettings");

        builder.Services.AddSingleton<IRepository<UserSetting>, MongoDbRepository<UserSetting>>(_ => new(
            mongoDbSettings.ContainerNames.UserSettings,
            mongoDbSettings.DatabaseName, 
            mongoDbSettings.ConnectionString, 
            mongoDbSettings.ForcePartitionKey, 
            environment));

        builder.Services.AddSingleton<IRepository<Workspace>, MongoDbRepository<Workspace>>(_ => new(
            mongoDbSettings.ContainerNames.Workspace,
            mongoDbSettings.DatabaseName, 
            mongoDbSettings.ConnectionString,
            mongoDbSettings.ForcePartitionKey,
            environment));

        builder.Services.AddSingleton<IRepository<Character>, MongoDbRepository<Character>>(_ => new(
            mongoDbSettings.ContainerNames.Character, 
            mongoDbSettings.DatabaseName, 
            mongoDbSettings.ConnectionString,
            mongoDbSettings.ForcePartitionKey,
            environment));

        builder.Services.AddSingleton<IRepository<Location>, MongoDbRepository<Location>>(_ => new(
            mongoDbSettings.ContainerNames.Location, 
            mongoDbSettings.DatabaseName, 
            mongoDbSettings.ConnectionString,
            mongoDbSettings.ForcePartitionKey,
            environment));

        builder.Services.AddSingleton<IRepository<Creature>, MongoDbRepository<Creature>>(_ => new(
            mongoDbSettings.ContainerNames.Creature, 
            mongoDbSettings.DatabaseName, 
            mongoDbSettings.ConnectionString,
            mongoDbSettings.ForcePartitionKey, 
            environment));

        // JSON Serialization Options
        builder.Services.AddSingleton(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Azure Blob Storage
        var blobStorageSettings = builder.Configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>()
            ?? throw new ArgumentException("Error getting BlobStorageSettings");

        // Services
        builder.Services.AddScoped<AzureBlobService>(_ => new(
            blobStorageSettings.ConnectionString, 
            blobStorageSettings.ContainerName));
        builder.Services.AddScoped<IIdentityProvider, UserProvider>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IUserSettingService, UserSettingService>();
        builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
        builder.Services.AddScoped<ICommonEntityService<Character>, CommonEntityService<Character>>();
        builder.Services.AddScoped<ICommonEntityService<Location>, CommonEntityService<Location>>();
        builder.Services.AddScoped<ICommonEntityService<Creature>, CommonEntityService<Creature>>();
        builder.Services.AddScoped<WorkspaceValidationService>();

        var app = builder.Build();

        // Exception handling 
        app.UseExceptionHandler();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseRoutes();

        app.Run();
    }
}
