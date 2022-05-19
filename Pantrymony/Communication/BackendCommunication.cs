using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;
using Pantrymony.Auth.Extensions;
using Pantrymony.Model;

namespace Pantrymony.Communication;

public static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(
        HttpClient client,
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        ILogger logger,
        Victual editedEntry)
    {
        string updateUrl =
            $"{configuration["TargetApi"]}/updatevictual?userId={editedEntry.UserId}&victualId={editedEntry.VictualId}";
        var updatePayload = JsonSerializer.Serialize(editedEntry, new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = false
        });
        logger.LogInformation("Sending PUT:[{UpdateUrl}]", updateUrl);
        logger.LogInformation("Sending {Content}:", updatePayload);


        using var request =
            await new HttpRequestMessage(HttpMethod.Put, updateUrl).AppendAuthorizationHeader(
                await Auth.Authentication.ReadIdToken(jsRuntime, logger));
        using var stringContent = new StringContent(updatePayload, Encoding.UTF8, "application/json");
        request.Content = stringContent;

        using var response = await client
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        logger.LogInformation("PUT-Response:\n{Response}", JsonSerializer.Serialize(response));
        logger.LogInformation("Response code: {Code}", response.StatusCode);
        logger.LogInformation("Response content: {Content}", await response.Content.ReadAsStringAsync());
    }

    public static async Task<IEnumerable<Unit>> FetchUnitsAsync(
        HttpClient client, 
        IConfiguration configuration,
        IJSRuntime jsRuntime,
        ILogger logger)
    {
        try
        {
            var getUrl = $"{configuration["AuthTargetApi"]}/units";
            logger.LogInformation("Fetching Units!");
            logger.LogInformation("Sending GET:[{Url}]", getUrl);

            var requestMsg =
                await new HttpRequestMessage(HttpMethod.Get, getUrl).AppendAuthorizationHeader(
                    await Auth.Authentication.ReadIdToken(jsRuntime, logger));
            var response = await client.SendAsync(requestMsg);
            logger.LogInformation("API responded with: {Response}", response);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<List<Unit>>();
                List<Unit> result = responseData != null
                    ? responseData.OrderBy(unit => unit.Symbol).ToList()
                    : new List<Unit>();
                result.ForEach(unit => logger.LogInformation("{Name}:{Symbol}", unit.Name, unit.Symbol));
                return result;
            }

            return new List<Unit>();
        }
        catch (Exception e)
        {
            logger.LogError("{Message}:\n{Stack}", e.Message, e.StackTrace);
            throw;
        }
    }
}