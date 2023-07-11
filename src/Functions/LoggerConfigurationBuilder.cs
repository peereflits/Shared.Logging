using Serilog;

namespace Peereflits.Shared.Logging;

internal static class LoggerConfigurationBuilder
{
    public static LoggerConfiguration UseDefault(string applicationName)
    {
        LoggerConfiguration result = new LoggerConfiguration()
                                    .MinimumLevel.Is(LogConfigurationBuilder.GetLogLevel());

        return result.UseDefaultConfiguration(applicationName);
    }
}