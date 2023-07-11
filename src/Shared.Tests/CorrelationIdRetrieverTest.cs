using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using NSubstitute;
using Xunit;

namespace Peereflits.Shared.Logging.Shared.Tests;

public class CorrelationIdRetrieverTest
{
    private const string HeaderKey = "x-correlation-id";
    private readonly IHttpContextAccessor contextAccessor;

    private readonly CorrelationIdRetriever subject;

    public CorrelationIdRetrieverTest()
    {
        contextAccessor = Substitute.For<IHttpContextAccessor>();
        subject = new CorrelationIdRetriever(contextAccessor);
    }

    [Fact]
    public void WhenRetrieveWithoutContext_ItShouldReturnANewId()
    {
        string result = subject.Execute();
        Assert.NotEqual(Guid.Empty, Guid.Parse(result));
    }

    [Fact]
    public void WhenRetrieveWithoutContext_ItShouldReturnANewIdForEachRequest()
    {
        string result1 = subject.Execute();
        string result2 = subject.Execute();
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void WhenRetrieveWhileContextHasId_ItShouldReturnThatOne()
    {
        var expected = Guid.NewGuid().ToString();

        var context = new DefaultHttpContext();
        context.Request.Headers.Add(HeaderKey, (StringValues)expected);
        contextAccessor.HttpContext.Returns(context);

        string result = subject.Execute();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void WhenRetrieveWhileContextHasId_ItShouldReturnThatOneForEachRequest()
    {
        var expected = Guid.NewGuid().ToString();

        var context = new DefaultHttpContext();
        context.Request.Headers.Add(HeaderKey, (StringValues)expected);
        contextAccessor.HttpContext.Returns(context);

        string result1 = subject.Execute();
        string result2 = subject.Execute();
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void WhenRetrieveWhileContextHasNoId_ItShouldGenerateOne()
    {
        contextAccessor.HttpContext.Returns(new DefaultHttpContext());

        string result = subject.Execute();
        Assert.NotEqual(Guid.Empty, Guid.Parse(result));
    }

    [Fact]
    public void WhenRetrieveWhileContextHasNoId_ItShouldGenerateOneAndReturnThatOneForEachRequest()
    {
        contextAccessor.HttpContext.Returns(new DefaultHttpContext());

        string result1 = subject.Execute();
        string result2 = subject.Execute();

        Assert.NotEqual(Guid.Empty, Guid.Parse(result1));
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void WhenRetrieve_ItShouldAddToResponse()
    {
        var context = new DefaultHttpContext();
        contextAccessor.HttpContext.Returns(context);

        string result = subject.Execute();

        Assert.True(context.Response.Headers.ContainsKey(HeaderKey));
        Assert.Equal(result, context.Response.Headers[HeaderKey]);
    }
}