using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Peereflits.Shared.Logging;

/// <summary>
///     <para>Reads from an <c>HttpContext</c> the correlation id if any.</para>
///     <para>If not found, a new correlation id is generated and added to the <c>HttpContext</c>.</para>
/// </summary>
public interface IRetrieveCorrelationId
{
    /// <summary>
    ///     Retrieves a correlation id.
    /// </summary>
    /// <returns>A <see langword="string" />.</returns>
    string Execute();
}

public class CorrelationIdRetriever : IRetrieveCorrelationId
{
    public const string HeaderKey = "x-correlation-id";

    private readonly IHttpContextAccessor contextAccessor;

    public CorrelationIdRetriever(IHttpContextAccessor contextAccessor) => this.contextAccessor = contextAccessor;

    public string Execute()
    {
        IHeaderDictionary? headers = contextAccessor.HttpContext?.Response.Headers;

        if(headers == null)
        {
            return Guid.NewGuid().ToString();
        }

        var value = string.Empty;
        if(contextAccessor.HttpContext?.Request.Headers.TryGetValue(HeaderKey, out StringValues source) ?? false)
        {
            value = source.FirstOrDefault();
        }
        else if(headers.TryGetValue(HeaderKey, out source))
        {
            value = source.FirstOrDefault();
        }

        string result = string.IsNullOrEmpty(value) ? Guid.NewGuid().ToString() : value;

        if(!headers.ContainsKey(HeaderKey))
        {
            headers.Append(HeaderKey, (StringValues)result);
        }

        return result;
    }
}