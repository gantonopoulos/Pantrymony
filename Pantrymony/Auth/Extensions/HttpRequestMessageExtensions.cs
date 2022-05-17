using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Pantrymony.Pages;

namespace Pantrymony.Auth.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static async Task<HttpRequestMessage> AppendAuthorizationHeader(
        this HttpRequestMessage request,
        IAccessTokenProvider accessTokenProvider, 
        ILogger logger)
    {
        request.Headers.Add("Authorization",
            "Bearer " + await Auth.GetAccessTokenJwtAsync(accessTokenProvider, logger));
        return request;
    }
}