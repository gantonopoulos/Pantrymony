using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Pantrymony.Auth;

public static class Auth
{
    public static async Task<string> GetAccessTokenJwtAsync(IAccessTokenProvider tokenProvider,ILogger logger)
    {
        var tokenResult = await tokenProvider.RequestAccessToken();
        tokenResult.TryGetToken(out AccessToken token);
        logger.LogInformation("Access Token:{Token}", token.Value);
        return token.Value;
    }

    
}