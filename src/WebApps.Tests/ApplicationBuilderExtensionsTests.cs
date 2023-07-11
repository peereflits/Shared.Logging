using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Xunit;

namespace Peereflits.Shared.Logging.WebApps.Tests;

public class ApplicationBuilderExtensionsTests
{
    [Fact]
    public void WhenUseRequestLogging_ItShouldAddSerilogRequestLogging()
    {
        var app = Substitute.For<IApplicationBuilder>();
        app.Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>()).Returns(app);

        var result = app.UseRequestLogging();

        app.Received().Use(Arg.Any<Func<RequestDelegate, RequestDelegate>>());
        Assert.Equal(result, app);
    }
}