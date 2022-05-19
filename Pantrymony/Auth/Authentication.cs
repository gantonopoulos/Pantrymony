using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;
using Pantrymony.Common;
using Pantrymony.Extensions;

namespace Pantrymony.Auth;

public static class Authentication
{
    public static async Task<string> ReadIdToken(IJSRuntime jsRuntime, ILogger logger)
    {
        try
        {
            const string key = "Microsoft.AspNetCore.Components.WebAssembly.Authentication.CachedAuthSettings";
            var authSettingsRaw = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
            var authSettings = JsonSerializer.Deserialize<CachedAuthSettings>(authSettingsRaw)
                .ThrowIfNull(new Exception("CachedAuthSettings deserialization failed."));
            var userOidRaw = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", authSettings.OidcUserKey);
            var user = JsonSerializer.Deserialize<AuthenticationTokenInfo>(userOidRaw)
                .ThrowIfNull(new Exception($"Oidc [{authSettings.OidcUserKey}] data deserialization failed!"));
            logger.LogInformation("Token: {Token}", user.IdToken);
            return user.IdToken.ThrowIfNull(new Exception("ID Token not found!"));
        }
        catch (Exception e)
        {
            logger.LogError("{Message}:{Stack}", e.Message,e.StackTrace);
            throw;
        }
    }

    private class CachedAuthSettings
    {
        [JsonPropertyName("authority")]
        public string Authority { get; set; }
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        public string OidcUserKey => $"oidc.user:{Authority}:{ClientId}";
    }
    
    private class AuthenticationTokenInfo
    {
        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }
        [JsonPropertyName("expires_at")]
        public int ExpiresAt { get; set; }
    }
}