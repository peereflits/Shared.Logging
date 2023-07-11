using System.Linq;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Peereflits.Shared.Logging;

/// <summary>
///     <para>Contains a extension method for registering logging on the function host level (Startup.cs).</para>
///     <para>These methods therefore only apply to Azure Function apps.</para>
/// </summary>
public static class FunctionsHostBuilderExtensions
{
    /// <summary>
    ///     Sets a default logging configuration as the host's default and registers the services.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IFunctionsHostBuilder" /> to set the logging configuration for./</param>
    /// <param name="applicationName">The name of the application.</param>
    /// <returns>The configured <see cref="IServiceCollection" />.</returns>
    /// <remarks>Azure Functions without a name will have an ApplicationName of "Microsoft.Azure.WebJobs.Script.WebHost".</remarks>
    public static IServiceCollection UseSerilog(this IFunctionsHostBuilder hostBuilder, string applicationName)
    {
        var logger = hostBuilder.Services.FirstOrDefault(s => s.ServiceType == typeof(ILogger<>));
        if(logger != null)
        {
            hostBuilder.Services.Remove(logger);
        }
        
        LoggerConfiguration config = LoggerConfigurationBuilder.UseDefault(applicationName);
        hostBuilder.Services.AddLogging(lb => lb.AddSerilog(config.CreateLogger()));

        return hostBuilder.Services;
    }
}