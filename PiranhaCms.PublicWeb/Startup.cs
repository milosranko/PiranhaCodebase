using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AttributeBuilder;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;
using PiranhaCMS.Common;
using PiranhaCMS.PublicWeb.Business.Filters;
using PiranhaCMS.PublicWeb.Models.Blocks;
using PiranhaCMS.PublicWeb.Models.Pages;
using PiranhaCMS.Search.Models.Enums;
using PiranhaCMS.Search.Startup;
using PiranhaCMS.Validators.Startup;
using System;
using System.IO;

namespace PiranhaCMS.PublicWeb
{
    public class Startup
    {
        /// <summary>
        /// The application config.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The current configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Service setup
            services.AddPiranha(options =>
            {
                options.AddRazorRuntimeCompilation = true;
                options.UseFileStorage();
                options.UseImageSharp();
                options.UseManager();
                options.UseTinyMCE();
                options.UseMemoryCache();
                options.UseEF<SQLiteDb>(db =>
                    db.UseSqlite(Configuration.GetConnectionString("piranha")));
                options.UseIdentityWithSeed<IdentitySQLiteDb>(db =>
                    db.UseSqlite(Configuration.GetConnectionString("piranha")));
                //TODO Change this to use SQL Server DB
                //options.UseEF<SQLServerDb>(db =>
                //    db.UseSqlServer(Configuration.GetConnectionString("piranha")));
                //options.UseIdentityWithSeed<IdentitySQLServerDb>(db =>
                //    db.UseSqlServer(Configuration.GetConnectionString("piranha")));
            });

            //Validator services setup
            services.AddPiranhaValidators(options =>
            {
                options.UsePageValidation = true;
                options.UseSiteValidation = true;
            });

            //Search services setup
            services.AddPiranhaSearch(options =>
            {
                options.StorageType = IndexDirectory.FileSystem;
                options.IndexDirectory = Path.Combine(Environment.CurrentDirectory, "Index");
                options.DefaultAnalyzer = DefaultAnalyzer.English;
            });

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(PageContextActionFilter));
            });
            //.AddRazorOptions(options =>
            //{
            //    options.ViewLocationFormats.Add("/Views/Shared/YourLocation/{0}.cshtml");
            //})
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IApi api,
            ILogger<Startup> logger)
        {
            ServiceActivator.Configure(app.ApplicationServices);

            //HTTP 500 handler
            if (env.IsDevelopment())
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
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/404";
                    await next();
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // Initialize Piranha
            App.Init(api);

            //Added support for SVG files, Piranha doesn't recognize it as an Image so it needs to be Document
            if (!App.MediaTypes.Documents.ContainsExtension(".svg"))
                App.MediaTypes.Documents.Add(".svg", "image/svg+xml");

            //Custom blocks registration
            App.Blocks.Register<ServicesBlockGroup>();
            App.Blocks.Register<TeaserBlock>();
            App.Blocks.Register<SearchBlock>();

            //Configure validator
            app.UsePiranhaValidators(logger);

            // Configure cache level
            App.CacheLevel = Piranha.Cache.CacheLevel.Basic;

            // Build content types
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build()
                .DeleteOrphans();

            // Configure Tiny MCE
            EditorConfig.FromFile("tinymce-config.json");

            //Init Piranha Search
            app.UsePiranhaSearch(api, logger, options =>
            {
                options.ForceReindexing = true;
                options.UseTextHighlighter = true;
                options.UseFacets = false;
                options.Include = new[]
                {
                    typeof(ArticlePage)
                };
            });

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });
        }
    }
}
