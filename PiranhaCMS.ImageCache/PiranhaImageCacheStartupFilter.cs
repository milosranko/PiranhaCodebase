using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace PiranhaCMS.ImageCache;

public class PiranhaImageCacheStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.UseMiddleware<PiranhaImageCacheMiddleware>();
            next(app);
        };
    }
}
