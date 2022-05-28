using Pantrymony.Common;

namespace Pantrymony.Auth.Extensions;

internal static class HttpRequestMessageExtensions
{
    public static string AuthorizationToken = string.Empty;

    public static async Task<HttpRequestMessage> AppendAuthorizationHeader(this HttpRequestMessage request,
        PageInjectedDependencies injectedDependencies)
    {
        if (string.IsNullOrEmpty(AuthorizationToken) || TokenHasExpired(AuthorizationToken))
        {
            AuthorizationToken =
                await Authentication.ReadIdToken(injectedDependencies.JScriptRuntime, injectedDependencies.Logger);
        }

        request.Headers.Add("Authorization", $"Bearer {AuthorizationToken}");
        return request;
    }

    /// <summary>
    /// TODO: Implement
    /// </summary>
    /// <param name="authorizationToken"></param>
    /// <returns></returns>
    private static bool TokenHasExpired(string authorizationToken)
    {
        return false;
    }
}