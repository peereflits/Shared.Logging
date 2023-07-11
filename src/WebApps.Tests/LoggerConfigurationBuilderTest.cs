using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Peereflits.Shared.Logging.WebApps.Tests;

public class LoggerConfigurationBuilderTest
{
    [Fact]
    public void WhenBuild_ItShouldReturnAConfiguredLoggerConfiguration()
    {
        LoggerConfiguration config = LoggerConfigurationBuilder.Build("MyApplicationName");

        Assert.NotNull(config);
        using Logger l = config.CreateLogger();
        Assert.True(l.IsEnabled(LogEventLevel.Information));
    }

    [Fact]
    public void WhenUseDefault_ItShouldReturnAConfiguredLoggerConfiguration()
    {
        var config = new LoggerConfiguration();
        config.UseDefault("MyApplicationName");

        Assert.NotNull(config);
        using Logger l = config.CreateLogger();
        Assert.True(l.IsEnabled(LogEventLevel.Information));
    }
}