using Serilog;
using Serilog.Core;
using Serilog.Events;
using Xunit;

namespace Peereflits.Shared.Logging.Functions.Tests;

[Collection("EnvironmentVariables")]
public class LoggerConfigurationBuilderTest
{
    [Fact]
    public void WhenUseDefault_ItShouldReturnAConfiguredLoggerConfiguration()
    {
        LoggerConfiguration config = LoggerConfigurationBuilder.UseDefault("MyFunctionName");

        Assert.NotNull(config);
        using Logger l = config.CreateLogger();
        Assert.True(l.IsEnabled(LogEventLevel.Information));
    }
}