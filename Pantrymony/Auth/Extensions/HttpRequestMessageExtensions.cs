using Pantrymony.Common;

namespace Pantrymony.Auth.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static async Task<HttpRequestMessage> AppendAuthorizationHeader(this HttpRequestMessage request, PageInjectedDependencies injectedDependencies)
    {
        string idToken =
            await Authentication.ReadIdToken(injectedDependencies.JScriptRuntime, injectedDependencies.Logger);
        request.Headers.Add("Authorization", $"Bearer {idToken}" );
        return request;
    }
}