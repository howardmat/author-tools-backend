# Clean Orphaned Files - Azure Function

This Azure Function cleans up orphaned files from Azure Blob Storage on a timer schedule.

## Local Development Setup

### Required Configuration

This function requires the following configuration values:

#### User Secrets (for sensitive data)
Initialize user secrets if not already done:
```bash
dotnet user-secrets init --project CleanOrphanedFiles.csproj
```

Add the following secrets:
```bash
dotnet user-secrets set "BlobStorageSettings:ConnectionString" "<your-blob-connection-string>"
dotnet user-secrets set "MongoDbSettings:ConnectionString" "<your-mongo-connection-string>"
```

#### local.settings.json (for non-sensitive settings)
The following values are configured in `local.settings.json`:
- `AzureWebJobsStorage`: Local storage emulator
- `FUNCTIONS_WORKER_RUNTIME`: dotnet-isolated
- `KeyVaultName`: author-tools-kv
- `BlobStorageSettings__ConnectionString`: 
- `MongoDbSettings__ConnectionString`: 
- `MongoDbSettings__DatabaseName`: app
- `MongoDbSettings__ContainerNames__Character`: characters
- `MongoDbSettings__ContainerNames__Location`: locations
- `MongoDbSettings__ContainerNames__Creature`: creatures
- `MongoDbSettings__ForcePartitionKey`: false

**Note**: See `local.settings.template.json` for a complete list of required configuration values.

## How it works

The function runs on a timer schedule defined by a [cron expression](https://en.wikipedia.org/wiki/Cron#CRON_expression).