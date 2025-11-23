# Author Tools

A modern web application designed to help authors organize and manage their creative writing projects. Author Tools provides a centralized platform for tracking characters, locations, and world-building details across multiple story universes and book series.

## 🎯 Features

- **Story Universe Management**: Organize multiple book series and standalone stories in separate universes
- **Character Tracking**: Maintain detailed character profiles, relationships, and development arcs
- **Location/Setting Database**: Document and reference settings, locations, and world-building elements
- **Secure Authentication**: Protected workspace with user-specific data management

## 🏗️ Architecture

This is the backend application for Author Tools. The frontend UI is maintained in a separate repository:

**Frontend Repository**: [author-tools-frontend](https://github.com/howardmat/author-tools-frontend)

The application follows a client-server architecture where:
- **Frontend**: React SPA handling UI/UX, authentication, and client-side state management
- **Backend (this repo)**: RESTful API providing data persistence, business logic, and server-side operations

## 🚀 Tech Stack

### Core Framework
- **.NET 10** - Built using the latest .NET framework with minimal API architecture for lightweight, high-performance endpoints
- **C# 14** - Leveraging modern C# language features for clean, maintainable code

### Cloud Infrastructure (Azure)
- **Azure App Service** - Hosted as a scalable web application on Azure's PaaS offering
- **Azure Blob Storage** - Stores user-uploaded files (character images, etc.) with metadata management
- **Azure Key Vault** - Securely manages sensitive configuration values and connection strings in production
- **Azure Identity** - Integrated authentication using `DefaultAzureCredential` for seamless Azure service access

### Database
**[MongoDB](https://www.mongodb.com/)** - NoSQL document database for flexible schema design
  - Implements repository pattern with generic base classes
  - Uses partition key strategy for multi-tenant data isolation
  - Supports CRUD operations with PATCH endpoint capabilities
  - Camel case serialization convention for JSON compatibility

### Authentication & Authorization
- **[Clerk](https://clerk.com/)** - Modern authentication platform handling user identity
- **JWT Bearer Authentication** - Token-based security with custom claims (user ID, email, first name, last name)
- **Custom Endpoint Filters** - Extracts and manages user context from JWT tokens throughout the request pipeline

### Key Features
- **Multi-tenant Architecture** - Data isolation using partition keys based on user ID and environment
- **Generic Repository Pattern** - Reusable data access layer for common entities (Characters, Creatures, Locations)
- **Workspace Management** - Organize content into separate workspaces with validation rules
- **File Management** - Upload, retrieve, and delete files with metadata tracking
- **User Settings** - Personalized user preferences (theme, etc.)
- **Global Exception Handling** - Centralized error management with problem details responses
- **CORS Configuration** - Environment-specific origin allowlists for secure cross-origin requests

## 🏗️ Project Structure

```
AuthorTools.Api/          # Main API project with routes, services, and configuration
AuthorTools.Data/         # Data access layer with repositories and MongoDB models
AuthorTools.SharedLib/    # Shared utilities, extensions, and DTOs
```

## 🔧 Configuration

The application uses hierarchical configuration with environment-specific overrides:

- **Development**: Local configuration in `appsettings.Development.json`
- **Production**: Azure Key Vault integration for secure secrets management

Key configuration sections:
- JWT settings (issuer, audience, signing key)
- MongoDB connection and database settings
- CORS allowed origins
- Azure Storage connection strings

## 🚦 Getting Started

1. Clone the repository:
```bash
git clone https://github.com/howardmat/author-tools-backend.git
```
2. Configure user secrets or update `appsettings.Development.json` with:
   - MongoDB connection string
   - Azure Storage connection string
   - Clerk JWT configuration
3. Run the application: `dotnet run --project AuthorTools.Api`
4. Access Swagger UI at `https://localhost:7150/swagger`

## 🚀 Deployment

This project is configured for deployment to Azure App Services with automated CI/CD via GitHub Actions.

## 📄 License

This project is licensed under GNU General Public License v3.0 - see the COPYING file for details.

## 👤 Author

Mat Howard - [GitHub Profile](https://github.com/howardmat)