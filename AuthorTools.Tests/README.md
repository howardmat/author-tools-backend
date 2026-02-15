# AuthorTools.Tests

Test project for the solution.

## Structure
- `Api/Services/*` unit tests for service classes (use substitutes for repositories/identity providers)
- `Api/Validators/*` unit tests for validators and validation result helpers
- `Api/Mappers/*` unit tests for mapping extensions
- `Common/*` unit tests for common utilities
- `Data/*` prefer integration tests for `MongoDbRepository<T>` (requires Mongo), unless the repo is refactored for seam injection

## Running
- `dotnet test`
