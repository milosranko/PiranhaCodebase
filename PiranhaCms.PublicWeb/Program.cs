using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AttributeBuilder;
using Piranha.Cache;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;
using PiranhaCMS.Common;
using PiranhaCMS.Common.Extensions;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.ImageCache;
using PiranhaCMS.PublicWeb.Business.Filters;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Startup;
using PiranhaCMS.Validators.Startup;
using Serilog;
using System;
using System.IO;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

#region Configure logger

builder.Host.UseSerilog((ctx, provider, lc) => lc.ReadFrom.Configuration(ctx.Configuration));
builder.Logging.AddSerilog(new LoggerConfiguration().CreateLogger(), true);

#endregion

#region Configuration binding

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

#endregion

#region Services registration

builder.Services.AddTransient<IStartupFilter, PiranhaImageCacheStartupFilter>();

#region Piranha CMS

builder.Services
    .AddPiranha(options =>
    {
        options.AddRazorRuntimeCompilation = true;
        options.UseCms();
        options.UseManager();
        options.UseFileStorage();
        options.UseImageSharp();
        options.UseTinyMCE();
        options.UseMemoryCache();
        options.UseEF<SQLiteDb>(db =>
            db.UseSqlite(builder.Configuration.GetConnectionString("piranha")));
        options.UseIdentityWithSeed<IdentitySQLiteDb>(db =>
            db.UseSqlite(builder.Configuration.GetConnectionString("piranha")));
        //TODO Change this to use SQL Server DB
        //options.UseEF<SQLServerDb>(db =>
        //    db.UseSqlServer(Configuration.GetConnectionString("piranha")));
        //options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
        //    db.UseSqlServer(Configuration.GetConnectionString("piranha")));
    })
    .AddPiranhaValidators(options =>
    {
        options.UsePageValidation = true;
        options.UseSiteValidation = true;
    })
    .AddPiranhaSearch(options =>
    {
        options.StorageType = IndexDirectory.FileSystem;
        options.IndexDirectory = Path.Combine(Environment.CurrentDirectory, "Index");
        options.DefaultAnalyzer = DefaultAnalyzer.English;
    });

#endregion

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(PageContextActionFilter));
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

        context.Request.Path = "/404";
        context.Response.Redirect(context.Request.Path, true);
    }
});

#endregion

app.UseResponseCaching();
app.UseHttpsRedirection();

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
app.UsePiranhaValidators(typeof(StartPage).Assembly, app.Logger);

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
app.UsePiranhaSearch(api, app.Logger, options =>
{
    options.ForceReindexing = app.Configuration.GetRequiredSection("PiranhaSearch").GetValue<bool>("ForceReindexing");
    options.UseTextHighlighter = true;
    options.UseFacets = false;
    options.Include = new[]
    {
        typeof(ArticlePage)
    };
});

//Middleware setup
app.UsePiranha(options =>
{
    options.UseManager();
    options.UseTinyMCE();
    options.UseIdentity();
});

#endregion

app.Run();
