using Microsoft.Extensions.Configuration;
using Serilog;

namespace Peereflits.Shared.Logging;

internal static class LoggerConfigurationBuilder
{
    public static LoggerConfiguration Build(string applicationName, IConfiguration configuration = null) 
        => new LoggerConfiguration().UseDefault(applicationName, configuration);

    public static LoggerConfiguration UseDefault(this LoggerConfiguration loggerConfiguration, string applicationName, IConfiguration configuration = null)
    {
        if(configuration == null)
        {
            loggerConfiguration.MinimumLevel.Is(LogConfigurationBuilder.GetLogLevel());
        }
        else
        {
            loggerConfiguration.ReadFrom.Configuration(configuration);
        }

        return loggerConfiguration.UseDefaultConfiguration(applicationName);
    }
}