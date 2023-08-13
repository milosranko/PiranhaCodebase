using CmsContentBuilder.Piranha.Extensions;
using CmsContentBuilder.Piranha.Models;
using CmsContentBuilder.Piranha.Startup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.Data.EF.SQLite;
using Piranha.Extend.Blocks;
using PiranhaCMS.ContentTypes.Blocks;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.ContentTypes.Sites;
using PiranhaCMS.Validators.Startup;

namespace PiranhaCMS.Tests;

[TestClass]
public class PiranhaTests
{
    private const string HostUrl = "http://localhost:6001";

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        var builder = Host
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config
                .AddConfiguration(context.Configuration)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.unittest.json", true, true)
                .Build();
            })
            .ConfigureServices((context, services) =>
            {
                services
                .AddCmsContentBuilder(context.Configuration)
                .AddPiranhaValidators(options =>
                {
                    options.UsePageValidation = true;
                    options.UseSiteValidation = true;
                });
                Globals.Services = services.BuildServiceProvider();
            })
            .ConfigureWebHostDefaults(config =>
            {
                config.UseUrls(HostUrl);
                config.Configure(app =>
                {
                    app.UsePiranhaValidators(
                        typeof(StartPage).Assembly,
                        app.ApplicationServices.GetRequiredService<ILogger<PiranhaTests>>());
                    app.UseCmsContentBuilder(typeof(StartPage).Assembly,
                    builderOptions: o =>
                    {
                        o.DefaultLanguage = "sr-RS";
                        o.BuildMode = BuildModeEnum.Overwrite;
                        o.PublishContent = true;
                    },
                    builder: b =>
                    {
                        b
                        .WithSite<PublicSite>(s =>
                        {
                            s.SiteFooter.Column1Header = PropertyHelpers.AddRandomText();
                            s.SiteFooter.Column2Header = PropertyHelpers.AddRandomText();
                            s.SiteFooter.Column3Header = PropertyHelpers.AddRandomText();
                        })
                        .WithPage<StartPage>(p =>
                        {
                            p.Title = "StartPage";
                            p.PrimaryImage = PropertyHelpers.AddRandomImage(Globals.Services.GetRequiredService<IApi>());
                            p.Blocks
                            .Add<TeaserBlock>(block =>
                            {
                                block.Heading = PropertyHelpers.AddRandomText();
                            })
                            .Add<HtmlBlock>(block =>
                            {
                                block.Body = PropertyHelpers.AddRandomHtml();
                            });
                        })
                        .WithPage<ArticlePage>(p =>
                        {
                            p.Title = "Article1_1";
                            p.PageRegion.Heading = PropertyHelpers.AddRandomText();
                        }, l2 =>
                        {
                            l2
                            .WithSubPage<ArticlePage>(p =>
                            {
                                p.Title = "Article2_1";
                                p.PageRegion.Heading = PropertyHelpers.AddRandomText();
                            })
                            .WithSubPage<ArticlePage>(p =>
                            {
                                p.Title = "Article2_2";
                                p.PageRegion.Heading = PropertyHelpers.AddRandomText();
                            });
                        })
                        .WithPages<ArticlePage>(p =>
                        {
                            p.Title = "Article1_2";
                            p.PageRegion.Heading = PropertyHelpers.AddRandomText();
                        }, 100);
                    });
                });
            });

        builder.Build().Start();
    }

    [ClassCleanup]
    public static void Uninitialize()
    {
        #region DB cleanup

        var dbContext = Globals.Services.GetRequiredService<SQLiteDb>();
        dbContext.Database.EnsureDeleted();

        #endregion

        #region Files cleanup

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        if (Directory.Exists(path))
            Directory.Delete(path, true);

        #endregion
    }

    [TestMethod]
    public void InitializationTest()
    {
        //Arrange
        var api = Globals.Services.GetRequiredService<IApi>();

        //Act
        var pages = api.Pages.GetAllAsync().GetAwaiter().GetResult();
        var site = api.Sites.GetDefaultAsync().GetAwaiter().GetResult();
        var defaultLanguage = api.Languages.GetDefaultAsync().GetAwaiter().GetResult();

        //Assert
        Assert.IsNotNull(site);
        Assert.IsTrue(site.LanguageId.Equals(defaultLanguage.Id));
        Assert.IsNotNull(pages);
        Assert.IsTrue(pages.Count() > 100);
    }
}