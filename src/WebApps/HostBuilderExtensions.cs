using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Peereflits.Shared.Logging;

/// <summary>
///     <para>Contains extension methods for registering logging on the host level (Program.cs).</para>
///     <para>These methods therefore only apply to hosted apps (i.e. ASP.NET -core- apps on Azure WebApp or Kestrel).</para>
/// </summary>
/// <remarks>
///     For usage of Logging.Extensions in Azure Functions, please use the <c>Logging.Extensions.Functions</c> package.
/// </remarks>
public static class HostBuilderExtensions
{
    /// <summary>
    ///     Sets a default logging configuration as the host's default and registers the services.
    /// </summary>
    /// <param name="hostBuilder">The <see cref="IHostBuilder" /> to set the logging configuration for./</param>
    /// <param name="useApplicationConfiguration">
    ///     Indicates whether to use the application's <see cref="IConfiguration"/> as base for the <see cref="LoggerConfiguration"/>
    /// </param>
    /// <returns>The configured <paramref name="hostBuilder" />.</returns>
    /// <example>
    ///     The following example shows the usage in an ASP.NET (core) app:
    ///     <code>
    /// public static IHostBuilder CreateHostBuilder(string[] args) =>
    ///     Host.CreateDefaultBuilder(args)
    ///         .UseSerilog()
    ///         .ConfigureWebHostDefaults(webBuilder =>
    ///                                   {
    ///                                       webBuilder.UseStartup&lt;Startup&gt;();
    ///                                   });
    /// </code>
    /// </example>
    /// <example>
    ///     When using the application's <see cref="IConfiguration"/>, i.e. <c>AddSerilog(useApplicationConfiguration: true)</c>,
    ///     then the <c>appsettings.json</c> should look like:
    ///     <code>
    ///         {
    ///          "Serilog": {
    ///               "Using": [
    ///               "Serilog.Sinks.Seq"
    ///                       ],
    ///               "MinimumLevel": {
    ///                   "Default": "Debug",
    ///                   "Override": {
    ///                       "Microsoft": "Warning"
    ///                   }
    ///               },
    ///               "WriteTo": [
    ///               {
    ///                   "Name": "Seq",
    ///                   "Args": {
    ///                       "serverUrl": "http://localhost:5341",
    ///                       "apiKey": "none"
    ///                   }
    ///               }
    ///             ]
    ///           }
    ///         }
    ///     </code>
    /// </example>
    public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder, bool useApplicationConfiguration = false)
    {
        return hostBuilder.UseSerilog((context, configuration) =>
                                      {
                                          if(useApplicationConfiguration)
                                          {
                                              configuration.UseDefault(Application.Name, context.Configuration);
                                          }
                                          else
                                          {
                                              configuration.UseDefault(Application.Name);
                                          }
                                      });
    }
}