using CmsContentScaffolding.Piranha.Extensions;
using CmsContentScaffolding.Piranha.Models;
using CmsContentScaffolding.Piranha.Startup;
using CmsContentScaffolding.Shared.Resources;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.Data.EF.SQLite;
using Piranha.Extend.Blocks;
using PiranhaCMS.ContentTypes.Blocks;
using PiranhaCMS.ContentTypes.Pages;
using PiranhaCMS.ContentTypes.Sites;
using PiranhaCMS.ImageCache.Models;
using PiranhaCMS.ImageCache.Startup;
using PiranhaCMS.Validators.Startup;

namespace PiranhaCMS.Tests;

[TestClass]
public class PiranhaTests
{
    private const string HostUrl = "http://localhost:6001";

    [ClassInitialize]
    public static void Initialize(TestContext context)
    {
        var builder = WebHost
            .CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) =>
            {
                config
                .AddConfiguration(context.Configuration)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.unittest.json", false, true)
                .Build();
            })
            .ConfigureServices((context, services) =>
            {
                services
                .AddCmsContentScaffolding(context.Configuration)
                .AddImageCache()
                .AddPiranhaValidators(options =>
                {
                    options.UsePageValidation = true;
                    options.UseSiteValidation = true;
                });
                Globals.Services = services.BuildServiceProvider();
            })
            .Configure(builder =>
            {
                builder.UsePiranhaValidators(typeof(StartPage).Assembly);
                builder.UseImageCache(o =>
                {
                    o.ConvertToWebP = true;
                    o.Quality = ImageQuality.Medium;
                });
                builder.UseCmsContentScaffolding(typeof(StartPage).Assembly,
                builderOptions: o =>
                {
                    o.DefaultLanguage = "sr-RS";
                    o.BuildMode = BuildModeEnum.Overwrite;
                    o.PublishContent = true;
                },
                builder: b =>
                {
                    b.UsePages()
                    .WithSite<PublicSite>(s =>
                    {
                        s.SiteFooter.Column1Header = ResourceHelpers.Faker.Lorem.Paragraphs();
                        s.SiteFooter.Column2Header = ResourceHelpers.Faker.Lorem.Paragraphs();
                        s.SiteFooter.Column3Header = ResourceHelpers.Faker.Lorem.Paragraphs();
                    })
                    .WithPage<StartPage>(p =>
                    {
                        p.Title = "Start Page";
                        p.PrimaryImage = PropertyHelpers.AddRandomImage(Globals.Services.GetRequiredService<IApi>(), "PrimaryImage.png", ResourceHelpers.GetImageStream());
                        p.Blocks
                        .Add<TeaserBlock>(block =>
                        {
                            block.Heading = ResourceHelpers.Faker.Lorem.Slug();
                            block.MainText = ResourceHelpers.Faker.Lorem.Paragraphs();
                        })
                        .Add<HtmlBlock>(block =>
                        {
                            block.Body = ResourceHelpers.Faker.Lorem.Paragraphs();
                        });
                    })
                    .WithPage<ArticlePage>(p =>
                    {
                        p.Title = "Article1_1";
                        p.PageRegion.Heading = ResourceHelpers.Faker.Lorem.Slug();
                    }, l2 =>
                    {
                        l2
                        .WithPage<ArticlePage>(p =>
                        {
                            p.Title = "Article2_1";
                            p.PageRegion.Heading = ResourceHelpers.Faker.Lorem.Slug();
                        })
                        .WithPage<ArticlePage>(p =>
                        {
                            p.Title = "Article2_2";
                            p.PageRegion.Heading = ResourceHelpers.Faker.Lorem.Slug();
                        });
                    })
                    .WithPages<ArticlePage>(p =>
                    {
                        p.Title = "Article1_2";
                        p.PageRegion.Heading = ResourceHelpers.Faker.Lorem.Slug();
                    }, 100);
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