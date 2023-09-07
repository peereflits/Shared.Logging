using System;
using Destructurama;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Logz.Io;

namespace Peereflits.Shared.Logging;

public static class LogConfigurationBuilder
{
    internal const string LogToConsole = "Logging:LogTo:Console";
    internal const string LogToLogzio = "Logging:LogTo:Logzio";
    internal const string LogToAppInsights = "Logging:LogTo:ApplicationInsights";
    internal const string LogzioToken = "Logging:Logzio:Token";
    internal const string LogzioSubDomain = "Logging:Logzio:DataCenter:SubDomain";
    internal const string LogLevel = "Logging:LogLevel";

    public static LoggerConfiguration UseDefaultConfiguration(this LoggerConfiguration loggerConfiguration, string applicationName)
    {
        loggerConfiguration
               .Destructure.UsingAttributes()
               .Enrich.FromLogContext()
               .Enrich.WithMachineName()
               .Enrich.WithAssemblyName()
               .Enrich.WithAssemblyVersion()
               .Enrich.WithCorrelationIdHeader()
               .Enrich.WithProperty("EnvironmentName", GetEnvironment())
               .Enrich.WithProperty("ApplicationName", applicationName);

        if(IsDevelopment)
        {
            loggerConfiguration.WriteTo.Console();
            return loggerConfiguration;
        }

        if(ShouldLogToConsole())
        {
            loggerConfiguration.WriteTo.Console();
        }

        if(ShouldLogToLogzIo())
        {
            loggerConfiguration.WriteTo.LogzIo(GetLogzioToken(), "serilog", GetLogzioOptions());
        }

        if(ShouldLogToApplicationInsights())
        {
            loggerConfiguration.WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces);
        }

        return loggerConfiguration;
    }

    private static bool IsDevelopment => GetEnvironment() == "Development";

    private static string GetEnvironment() => Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT")
                                           ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                                           ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                                           ?? "Development";

    private static bool ShouldLogToConsole() =>
            (Environment.GetEnvironmentVariable(LogToConsole)?.ToLowerInvariant() ?? string.Empty) is "true" or "yes" or "1" or "y";

    private static bool ShouldLogToLogzIo() =>
            (Environment.GetEnvironmentVariable(LogToLogzio)?.ToLowerInvariant() ?? string.Empty) is "true" or "yes" or "1" or "y";

    private static bool ShouldLogToApplicationInsights() =>
            (Environment.GetEnvironmentVariable(LogToAppInsights)?.ToLowerInvariant() ?? string.Empty) is "true" or "yes" or "1" or "y";

    private static string GetLogzioToken()
    {
        if(IsDevelopment)
        {
            return string.Empty;
        }

        string? token = Environment.GetEnvironmentVariable(LogzioToken);

        return string.IsNullOrWhiteSpace(token)
                       ? throw new InvalidOperationException($"Missing a configuration value for '{LogzioToken}'")
                       : token;
    }

    private static LogzioOptions GetLogzioOptions() => new LogzioOptions
                                                       {
                                                           DataCenter = new LogzioDataCenter
                                                                        {
                                                                            SubDomain = Environment.GetEnvironmentVariable(LogzioSubDomain),
                                                                            UseHttps = true
                                                                        },
                                                           TextFormatterOptions = new LogzioTextFormatterOptions
                                                                                  {
                                                                                      BoostProperties = true,
                                                                                      FieldNaming = LogzIoTextFormatterFieldNaming.KeepAsIs
                                                                                  }
                                                       };

    public static LogEventLevel GetLogLevel()
    {
        string? level = Environment.GetEnvironmentVariable(LogLevel);

        return level?.ToLowerInvariant().Trim() switch
        {
            "fatal" => LogEventLevel.Fatal,
            "critical" => LogEventLevel.Fatal,
            "error" => LogEventLevel.Error,
            "warning" => LogEventLevel.Warning,
            "info" => LogEventLevel.Information,
            "information" => LogEventLevel.Information,
            "debug" => LogEventLevel.Debug,
            "trace" => LogEventLevel.Verbose,
            "verbose" => LogEventLevel.Verbose,
            _ => LogEventLevel.Information
        };
    }
}