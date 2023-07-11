using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Peereflits.Shared.Logging.Functions.Tests;

[Collection("EnvironmentVariables")]
public class SerilogInstallerHostBuilderExtensionsTests
{
    private const string environment = "AZURE_FUNCTIONS_ENVIRONMENT";

    [Fact]
    public void WhenUseDefaultSerilog_ItShouldRegisterALogger()
    {
        string env = Environment.GetEnvironmentVariable(environment);
        Environment.SetEnvironmentVariable(environment, "Development");

        var hb = Substitute.For<IFunctionsHostBuilder>();
        hb.Services.Returns(new ServiceCollection());

        _ = hb.UseSerilog("MyFunctionName");

        Assert.Contains(hb.Services, x => x.ServiceType == typeof(ILogger<>));
        Assert.Contains(hb.Services,
                        x => x.ServiceType == typeof(ILoggerProvider)
                          && x.ImplementationInstance.ToString() == "Serilog.Extensions.Logging.SerilogLoggerProvider"
                       );

        Environment.SetEnvironmentVariable(environment, env);
    }
}