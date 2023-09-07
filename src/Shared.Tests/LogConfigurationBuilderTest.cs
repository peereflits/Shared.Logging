using System;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Peereflits.Shared.Logging.Shared.Tests;

[Collection("EnvironmentVariables")]
public class LogConfigurationBuilderTest
{
    private const string FunctionsEnvironment = "AZURE_FUNCTIONS_ENVIRONMENT";

    [Fact]
    public void WhenUseDefaultConfiguration_ItShouldReturnAConfiguredLoggerConfiguration()
    {
        LoggerConfiguration config = new LoggerConfiguration().UseDefaultConfiguration("MyApplicationName");

        Assert.NotNull(config);
        using Logger l = config.CreateLogger();
        Assert.True(l.IsEnabled(LogEventLevel.Information));
    }

    [Fact]
    public void WhenLogToLogzio_WhileTokenIsNotConfigured_ItShouldThrow()
    {
        string? env = Environment.GetEnvironmentVariable(FunctionsEnvironment);
        string? logTo = Environment.GetEnvironmentVariable(LogConfigurationBuilder.LogToLogzio);
        string? token = Environment.GetEnvironmentVariable(LogConfigurationBuilder.LogzioToken);

        Environment.SetEnvironmentVariable(FunctionsEnvironment, "Test");
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogToLogzio, "1");
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogzioToken, string.Empty);

        var ex = Assert.Throws<InvalidOperationException>(() => new LoggerConfiguration().UseDefaultConfiguration("MyApplicationName"));
        Assert.Equal("Missing a configuration value for 'Logging:Logzio:Token'", ex.Message);

        Environment.SetEnvironmentVariable(FunctionsEnvironment, env);
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogToLogzio, logTo);
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogzioToken, token);
    }

    [Fact]
    public void WhenBuild_WhileNotInDebug_ItShouldReturnAnInstance()
    {
        string? env = Environment.GetEnvironmentVariable(FunctionsEnvironment);
        string? logTo = Environment.GetEnvironmentVariable(LogConfigurationBuilder.LogToConsole);

        Environment.SetEnvironmentVariable(FunctionsEnvironment, "Test");
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogToConsole, "yes");

        LoggerConfiguration result = new LoggerConfiguration().UseDefaultConfiguration("MyApplicationName");
        Assert.NotNull(result);

        Environment.SetEnvironmentVariable(FunctionsEnvironment, env);
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogToConsole, logTo);
    }

    [Theory]
    [InlineData("Fatal", LogEventLevel.Fatal)]
    [InlineData("Critical", LogEventLevel.Fatal)]
    [InlineData("Error", LogEventLevel.Error)]
    [InlineData("Warning", LogEventLevel.Warning)]
    [InlineData("Info", LogEventLevel.Information)]
    [InlineData("Information", LogEventLevel.Information)]
    [InlineData("Debug", LogEventLevel.Debug)]
    [InlineData("Trace", LogEventLevel.Verbose)]
    [InlineData("Verbose", LogEventLevel.Verbose)]
    [InlineData("any", LogEventLevel.Information)]
    public void WhenConfiguring_ItShouldSetCorrectLogLevel(string level, LogEventLevel expected)
    {
        string? logLevel = Environment.GetEnvironmentVariable(LogConfigurationBuilder.LogLevel);
        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogLevel, level);

        LogEventLevel actual = LogConfigurationBuilder.GetLogLevel();

        Assert.Equal(expected, actual);

        Environment.SetEnvironmentVariable(LogConfigurationBuilder.LogLevel, logLevel);
    }
}