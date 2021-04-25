using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    { }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseElapsed();

        app.Run(async context =>
        {
            await Task.Delay(500);
            await context.Response.WriteAsync("Elapsed Middleware Example");
        });
    }
}

public class ElapsedMiddleware
{
    readonly RequestDelegate _next;
    readonly Stopwatch _timer;

    public ElapsedMiddleware(RequestDelegate next)
    {
        _next = next;
        _timer = new Stopwatch();
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            _timer.Stop();
            context.Response.Headers["X-Elapsed"] = _timer.Elapsed.ToString();

            return Task.CompletedTask;
        });

        _timer.Restart();
        await _next(context);
    }
}

public static class ElapsedMiddlewareExtensioins
{
    public static IApplicationBuilder UseElapsed(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ElapsedMiddleware>();
    }
}