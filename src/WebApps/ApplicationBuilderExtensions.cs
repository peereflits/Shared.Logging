using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Peereflits.Shared.Logging;

/// <summary>
///     <para>Contains extension methods for using request logging for web apps.</para>
///     <para>These methods therefore only apply to hosted apps (i.e. ASP.NET -core- apps on Azure WebApp or Kestrel).</para>
/// </summary>
/// <remarks>
///     For usage of Logging.Extensions in Azure Functions, please use the <c>Logging.Extensions.Functions</c> package.
/// </remarks>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds middleware for Serilog request logging.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> instance to use.</param>
    /// <returns>The configured <paramref name="app"/> instance.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseSerilogRequestLogging(o => o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                                                                             {
                                                                                 diagnosticContext.Set("RequestUserAgent", httpContext.Request.Headers["User-Agent"]);
                                                                             });
    }
}