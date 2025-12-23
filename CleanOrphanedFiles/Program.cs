using AuthorTools.Api.Services;
using AuthorTools.Common.Options;
using AuthorTools.Data.Models;
using AuthorTools.Data.Repositories;
using AuthorTools.Data.Repositories.Interfaces;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

if (builder.Environment.IsProduction())
{
    var keyVaultName = builder.Configuration.GetSection("KeyVaultName").Value
        ?? throw new Exception($"Failed to read appsetting {JsonSerializer.Serialize(builder.Configuration)}");

    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Repos
var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
    ?? throw new ArgumentException("Error getting MongoDbSettings");

builder.Services.AddSingleton<IRepository<Character>, MongoDbRepository<Character>>(_ => new(
    mongoDbSettings.ContainerNames.Character, 
    mongoDbSettings.DatabaseName, 
    mongoDbSettings.ConnectionString, 
    mongoDbSettings.ForcePartitionKey));

builder.Services.AddSingleton<IRepository<Location>, MongoDbRepository<Location>>(_ => new(
    mongoDbSettings.ContainerNames.Location, 
    mongoDbSettings.DatabaseName, 
    mongoDbSettings.ConnectionString,
    mongoDbSettings.ForcePartitionKey));

builder.Services.AddSingleton<IRepository<Creature>, MongoDbRepository<Creature>>(_ => new(
    mongoDbSettings.ContainerNames.Creature, 
    mongoDbSettings.DatabaseName, 
    mongoDbSettings.ConnectionString,
    mongoDbSettings.ForcePartitionKey));

// Azure Blob Storage
var blobStorageSettings = builder.Configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>()
    ?? throw new ArgumentException("Error getting BlobStorageSettings");

// Services
builder.Services.AddScoped<AzureBlobService>(_ => new(
    blobStorageSettings.ConnectionString,
    blobStorageSettings.ContainerName));

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
