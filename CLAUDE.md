# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Technology Stack

- **Framework**: ASP.NET Core 8.0 (C#)
- **CMS**: Piranha CMS 12.0
- **Database**: SQLite (development), SQLite/SQL Server (production)
- **Search**: Lucene.NET with custom search implementations
- **Storage**: Azure Blob Storage (production), Local file system (development)
- **Image Processing**: ImageSharp
- **Testing**: MSTest
- **Logging**: Serilog with Application Insights

## Development Commands

```bash
# Build the solution
dotnet build

# Run the web application
cd PiranhaCms.PublicWeb
dotnet run

# Run tests
dotnet test

# Clean and rebuild
dotnet clean
dotnet build

# Watch for changes during development
cd PiranhaCms.PublicWeb
dotnet watch run

# Run specific test project
cd PiranhaCMS.Tests
dotnet test

# Package restore
dotnet restore
```

## Architecture Overview

### Solution Structure

The solution follows a modular architecture with distinct projects for different concerns:

- **PiranhaCMS.PublicWeb**: Main web application hosting Piranha CMS with custom controllers, views, and API endpoints
- **PiranhaCMS.ContentTypes**: Defines all CMS content types (pages, blocks, regions, sites) using Piranha's attribute-based system
- **PiranhaCMS.Business**: Business logic layer containing services like OpenAI integration
- **PiranhaCMS.Search**: Custom Lucene.NET search implementation with support for both content and music library searching
- **PiranhaCMS.ImageCache**: Custom image caching and optimization middleware
- **PiranhaCMS.Validators**: Custom validation rules for Piranha content
- **PiranhaCMS.Common**: Shared utilities and extensions
- **PiranhaCMS.Tests**: Unit tests using MSTest framework

### Key Architectural Patterns

1. **Content Type Registration**: Content types are defined in PiranhaCMS.ContentTypes using attributes and auto-registered via ContentTypeBuilder in Program.cs:271-274

2. **Dependency Injection**: Services are registered in Program.cs with specific patterns:
   - View model factories use IPageViewModelFactory<TPage, TViewModel> interface
   - Custom services registered in the Services registration region (Program.cs:80-213)

3. **Search Architecture**: Dual search engine setup:
   - Piranha content search configured at Program.cs:157-177
   - Music library search configured at Program.cs:178-197
   - Both use Lucene indexes with configurable storage (Azure or local)

4. **Configuration Management**:
   - Environment-specific settings via appsettings.{Environment}.json
   - Azure Key Vault integration for production secrets (Program.cs:39-42)
   - Connection strings and storage configuration vary by environment

5. **MVC Pattern with ViewModels**: 
   - Controllers in PiranhaCms.PublicWeb/Controllers use IModelLoader
   - ViewModels created via factories or constructors
   - Response caching applied at controller action level

6. **Startup Configuration**: 
   - ServiceActivator pattern for service location (Program.cs:217)
   - Middleware pipeline configured with Piranha-specific components
   - Custom error handling for 404/500 responses

### Environment-Specific Behavior

- **Production**: Uses Azure services (Blob Storage, Key Vault), enhanced logging to Azure
- **Development**: Local file storage, console/file logging, EF query logging enabled