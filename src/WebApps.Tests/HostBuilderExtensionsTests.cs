using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Extensions.Logging;
using Xunit;

namespace Peereflits.Shared.Logging.WebApps.Tests;

public class HostBuilderExtensionsTests
{
    [Fact]
    public void WhenUseSerilog_ItShouldRegisterALogger()
    {
        IHostBuilder builder = Host.CreateDefaultBuilder();
        
        builder.UseSerilog();
        
        IHost host = builder.Build();
        object? logger = host.Services.GetService(typeof(ILogger<HostBuilderExtensionsTests>));
        Assert.NotNull(logger);

        object? loggerFactory = host.Services.GetService(typeof(ILoggerFactory));
        Assert.NotNull(loggerFactory);
        Assert.IsType<SerilogLoggerFactory>(loggerFactory);
    }

    [Fact]
    public void WhenUseDefaultWithConfiguration_ItShouldRegisterALogger()
    {
        IHostBuilder hb = Host.CreateDefaultBuilder()
                              .ConfigureAppConfiguration((context, builder) =>
                                                         {
                                                             builder.AddJsonFile("appSettings.json");
                                                         });
        
        hb.UseSerilog(useApplicationConfiguration: true);
        
        IHost host = hb.Build();
        object? logger = host.Services.GetService(typeof(ILogger<HostBuilderExtensionsTests>));
        Assert.NotNull(logger);

        object? loggerFactory = host.Services.GetService(typeof(ILoggerFactory));
        Assert.NotNull(loggerFactory);
        Assert.IsType<SerilogLoggerFactory>(loggerFactory);
    }
}