using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Pantrymony.Auth.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static Task<HttpRequestMessage> AppendAuthorizationHeader(
        this HttpRequestMessage request,string idToken)
    {
        request.Headers.Add("Authorization", $"Bearer {idToken}" );
        return Task.FromResult(request);
    }
}