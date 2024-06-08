using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.AttributeBuilder;
using Piranha.Cache;
using Piranha.Data.EF.SQLite;
using Piranha.Data.EF.SQLServer;
using Piranha.Manager.Editor;
using PiranhaCMS.Business.OpenAi;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Common;
using PiranhaCMS.Common.Extensions;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.ImageCache.Startup;
using PiranhaCMS.PublicWeb.Api;
using PiranhaCMS.PublicWeb.Api.Services;
using PiranhaCMS.PublicWeb.Filters;
using PiranhaCMS.PublicWeb.Models.ViewModelFactories;
using PiranhaCMS.PublicWeb.Models.ViewModelFactories.Base;
using PiranhaCMS.PublicWeb.Models.ViewModels;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Startup;
using PiranhaCMS.Validators.Startup;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

#region Configuration binding

var configBuilder = builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

if (builder.Environment.IsProduction())
{
    configBuilder.AddAzureKeyVault(new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"), new DefaultAzureCredential());
}

configBuilder.Build();

#endregion

#region Logger

//builder.Host.UseSerilog((ctx, provider, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

if (builder.Environment.IsProduction())
{
    builder.Logging.AddSerilog(
        new LoggerConfiguration()
        .WriteTo.AzureBlobStorage(
            new CompactJsonFormatter(),
            new BlobServiceClient(builder.Configuration["Piranha:StorageConnectionString"]),
            storageContainerName: "logs",
            storageFileName: "application.log",
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
        .CreateLogger(), true);
}
else
{
    builder.Logging.AddSerilog(
        new LoggerConfiguration()
        .WriteTo.Console(new CompactJsonFormatter())
        .WriteTo.File(
            new CompactJsonFormatter(),
            "./logs/application.log",
            rollingInterval: RollingInterval.Hour,
            restrictedToMinimumLevel: LogEventLevel.Debug)
        .CreateLogger(), true);
}

#endregion

#region Services registration

if (builder.Environment.IsProduction())
{
    builder.Services.AddApplicationInsightsTelemetry(options =>
    {
        //Uses too much data
        options.EnableDependencyTrackingTelemetryModule = false;
    });
}

builder.Services
    //.AddTransient<IStartupFilter, PiranhaImageCacheStartupFilter>()
    .AddScoped<IPageViewModelFactory<MusicSearchPage, MusicSearchPageViewModel>, MusicSearchPageViewModelFactory>()
    .AddTransient<IApiService, ApiService>();

#region OpenAI

builder.Services
    .Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.Position))
    .AddOpenAiApi(builder.Configuration);

#endregion

#region Piranha CMS

builder.Services
    .AddPiranha(options =>
    {
        options.AddRazorRuntimeCompilation = true;
        options.UseCms();
        options.UseManager();
        if (builder.Environment.IsProduction())
        {
            options.UseBlobStorage(new Uri($"https://{builder.Configuration["BlobStorageName"]}.blob.core.windows.net/{builder.Configuration["Piranha:UploadsContainerName"]}"), new DefaultAzureCredential());
        }
        //else if (builder.Environment.IsDevelopment())
        //{
        //    options.UseBlobStorage(builder.Configuration["Piranha:StorageConnectionString"]);
        //}
        else
        {
            options.UseFileStorage();
        }
        options.UseImageSharp();
        options.UseTinyMCE();
        options.UseMemoryCache();
        if (builder.Environment.IsProduction())
        {
            options.UseEF<SQLServerDb>(db =>
            db.UseSqlServer(builder.Configuration.GetConnectionString("piranha-sql")));
            options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                db.UseSqlServer(builder.Configuration.GetConnectionString("piranha-sql")));
        }
        else
        {
            options.UseEF<SQLiteDb>(db =>
                db.UseSqlite(builder.Configuration.GetConnectionString("piranha")));
            options.UseIdentityWithSeed<IdentitySQLiteDb>(db =>
                db.UseSqlite(builder.Configuration.GetConnectionString("piranha")));
        }
    })
    .AddPiranhaValidators(options =>
    {
        options.UsePageValidation = true;
        options.UseSiteValidation = true;
    })
    .AddPiranhaSearch(options =>
    {
        if (builder.Environment.IsProduction())
        {
            options.StorageType = IndexDirectory.Azure;
            options.AzureStorageCredentials = builder.Configuration["Piranha:StorageConnectionString"];
            options.IndexDirectory = "piranha-lucene";
        }
        //else if (builder.Environment.IsDevelopment())
        //{
        //    options.StorageType = IndexDirectory.Azure;
        //    options.AzureStorageCredentials = builder.Configuration["Piranha:StorageConnectionString"];
        //    options.IndexDirectory = "piranha-lucene";
        //}
        else
        {
            options.StorageType = IndexDirectory.FileSystem;
            options.IndexDirectory = Path.Combine(Environment.CurrentDirectory, "indexes", "piranha");
        }
        options.DefaultAnalyzer = DefaultAnalyzer.English;
    })
    .AddMusicSearch(options =>
    {
        if (builder.Environment.IsProduction())
        {
            options.StorageType = IndexDirectory.Azure;
            options.AzureStorageCredentials = builder.Configuration["Piranha:StorageConnectionString"];
            options.IndexDirectory = "music-lucene";
        }
        //else if (builder.Environment.IsDevelopment())
        //{
        //    options.StorageType = IndexDirectory.Azure;
        //    options.AzureStorageCredentials = builder.Configuration["Piranha:StorageConnectionString"];
        //    options.IndexDirectory = "music-lucene";
        //}
        else
        {
            options.StorageType = IndexDirectory.FileSystem;
            options.IndexDirectory = Path.Combine(Environment.CurrentDirectory, "indexes", "music-library");
        }
    })
    .AddImageCache();

#endregion

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<PageContextActionFilter>();
});
//.AddRazorOptions(options =>
//{
//    options.ViewLocationFormats.Add("/Views/Shared/YourLocation/{0}.cshtml");
//})

builder.Services.AddResponseCaching();

#endregion

var app = builder.Build();

ServiceActivator.Configure(app.Services);

#region HTTP 500 and 404 handlers

//HTTP 500 handler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/500.html");
    app.UseHsts();
}

//HTTP 404 handler
app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == (int)HttpStatusCode.NotFound)
    {
        context.RequestServices.GetRequiredService<ILogger<Program>>().LogInformation("Page not found: " + context.Request.Path.Value);
        context.Request.Path = "/404";
        context.Response.Redirect(context.Request.Path, true);
    }
});

#endregion

app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseApiEndpoints();

#region Piranha init

using var scope = app.Services.CreateScope();
var api = scope.ServiceProvider.GetRequiredService<IApi>();

App.Init(api);

//Added support for SVG files, Piranha doesn't recognize it as an Image so it needs to be Document
if (!App.MediaTypes.Documents.ContainsExtension(".svg"))
    App.MediaTypes.Documents.Add(".svg", "image/svg+xml");

//Custom blocks registration
App.Blocks.AutoRegisterBlocks(typeof(StartPage).Assembly);

//Configure validator
app.UsePiranhaValidators(typeof(StartPage).Assembly);

//Configure cache level
App.CacheLevel = CacheLevel.Basic;

//Build content types
new ContentTypeBuilder(api)
    .AddAssembly(typeof(StartPage).Assembly)
    .Build()
    .DeleteOrphans();

//Configure Tiny MCE
EditorConfig.FromFile("tinymce-config.json");

//Init Piranha Search
app.UsePiranhaSearch(api, options =>
{
    options.ForceReindexing = app.Configuration.GetRequiredSection("PiranhaSearch").GetValue<bool>("ForceReindexing");
    options.UseTextHighlighter = true;
    options.UseFacets = false;
    options.Include =
    [
        typeof(ArticlePage)
    ];
});
app.UseMusicSearch();
app.UseImageCache(options => options.ConvertToWebP = true);

//Middleware setup
app.UsePiranha(options =>
{
    options.UseManager();
    options.UseTinyMCE();
    options.UseIdentity();
});

#endregion

app.Run();
