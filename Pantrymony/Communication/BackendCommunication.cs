using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Pantrymony.Auth.Extensions;
using Pantrymony.Common;
using Pantrymony.Extensions;
using Pantrymony.Model;

namespace Pantrymony.Communication;

internal static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(
        PageInjectedDependencies injectedDependencies,
        Victual editedEntry)
    {
        string updateUrl =
            $"{injectedDependencies.Configuration["TargetApi"]}/updatevictual?userId={editedEntry.UserId}&victualId={editedEntry.VictualId}";
        var updatePayload = JsonSerializer.Serialize(editedEntry, new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = false
        });
        injectedDependencies.Logger.LogInformation("Sending PUT:[{UpdateUrl}]", updateUrl);
        injectedDependencies.Logger.LogInformation("Sending {Content}:", updatePayload);


        using var request =
            await new HttpRequestMessage(HttpMethod.Put, updateUrl).AppendAuthorizationHeader(injectedDependencies);
        using var stringContent = new StringContent(updatePayload, Encoding.UTF8, "application/json");
        request.Content = stringContent;

        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        injectedDependencies.Logger.LogInformation("PUT-Response:\n{Response}", JsonSerializer.Serialize(response));
        injectedDependencies.Logger.LogInformation("Response code: {Code}", response.StatusCode);
        injectedDependencies.Logger.LogInformation("Response content: {Content}", await response.Content.ReadAsStringAsync());
    }

    public static async Task<IEnumerable<Unit>> FetchUnitsAsync(PageInjectedDependencies injectedDependencies)
    {
        try
        {
            var getUrl = $"{injectedDependencies.Configuration["TargetApi"]}/units";
            injectedDependencies.Logger.LogInformation("Fetching Units!");
            injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);

            var requestMsg =
                await new HttpRequestMessage(HttpMethod.Get, getUrl).AppendAuthorizationHeader(injectedDependencies);
            var response = await injectedDependencies.HttpClient.SendAsync(requestMsg);
            injectedDependencies.Logger.LogInformation("API responded with: {Response}", response);
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<List<Unit>>();
                var result = responseData != null
                    ? responseData.OrderBy(unit => unit.Symbol).ToList()
                    : new List<Unit>();
                result.ForEach(unit => 
                    injectedDependencies.Logger
                        .LogInformation("{Name}:{Symbol}", unit.Name, unit.Symbol));
                return result;
            }

            return new List<Unit>();
        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("{Message}:\n{Stack}", e.Message, e.StackTrace);
            throw;
        }
    }
    
    public static async Task<Victual> FetchVictualOfUser(
        string userId, 
        string victualId, 
        PageInjectedDependencies injectedDependencies)
    {
        try
        {
            var getUrl =
                $"{injectedDependencies.Configuration["TargetApi"]}/uservictual?userId={userId}&victualId={victualId}";
            injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);
            var requestMsg =
                await new HttpRequestMessage(HttpMethod.Get, getUrl).AppendAuthorizationHeader(injectedDependencies);
            var response = await injectedDependencies.HttpClient.SendAsync(requestMsg);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            injectedDependencies.Logger.LogInformation("API responded with: {Response}:\n{Content}", response,
                responseContent);
            return (await response.Content.ReadFromJsonAsync<List<Victual>>())
                .ThrowIfNull(new Exception($"Victual with id:{victualId} not found!"))
                .Single();
        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("{Message}:\n{Stack}", e.Message, e.StackTrace);
            throw;
        }
    }
    
    public static async Task<IEnumerable<Victual>> FetchVictualsAsync(
        string userId,  
        PageInjectedDependencies injectedDependencies)
    {
        try
        {
            var getUrl = $"{injectedDependencies.Configuration["TargetApi"]}/uservictuals?userId={userId}";
            injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);
            injectedDependencies.Logger.LogInformation("Fetching Victuals!");

            var requestMsg = await new HttpRequestMessage(HttpMethod.Get, getUrl)
                .AppendAuthorizationHeader(injectedDependencies);
            var response = await injectedDependencies.HttpClient.SendAsync(requestMsg);
            injectedDependencies.Logger.LogInformation("API responded with: {Response}", response);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<List<Victual>>();
                var entries = responseData != null 
                    ? responseData.OrderBy(entry => entry.Name).ToList() 
                    : new List<Victual>();
                entries.ForEach(Console.WriteLine);
                return entries;
            }

            return new List<Victual>();
        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("Victuals could not be parsed!\n{Message}{Stack}", e.Message, e.StackTrace);
            throw;
        }
    }

    public static async Task DeleteVictualAsync(Victual victual, PageInjectedDependencies injectedDependencies)
    {
        var deleteUrl =
            $"{injectedDependencies.Configuration["TargetApi"]}/deletevictual?userId={victual.UserId}&victualId={victual.VictualId}";
        injectedDependencies.Logger.LogInformation("Sending DELETE:[{Url}]", deleteUrl);
        using var request = await new HttpRequestMessage(HttpMethod.Delete, deleteUrl)
            .AppendAuthorizationHeader(injectedDependencies);
        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
    }

    public static async Task PostNewEntryAsync(Victual editedEntry, PageInjectedDependencies injectedDependencies)
    {
        injectedDependencies.Logger.LogInformation("Adding Entry with id {Identifier}", editedEntry.VictualId.ToString());

        var postUrl = $"{injectedDependencies.Configuration["TargetApi"]}/createvictual";
        var postPayload = JsonSerializer.Serialize(editedEntry, new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            PropertyNameCaseInsensitive = false
        });
        injectedDependencies.Logger.LogInformation("Sending POST:[{Url}]", postUrl);
        using var request = await new HttpRequestMessage(HttpMethod.Post, postUrl)
            .AppendAuthorizationHeader(injectedDependencies);
        using var stringContent = new StringContent(postPayload, Encoding.UTF8, "application/json");
        request.Content = stringContent;
        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }


    public static async Task PostImageAsync(
        Victual imageOwner, 
        byte[] selectedVictualImage, 
        PageInjectedDependencies injectedDependencies)
    {
        var postUrl = $"{injectedDependencies.Configuration["TargetApi"]}/uploadImage?userId={imageOwner.UserId}&victualId={imageOwner.VictualId}";
        injectedDependencies.Logger.LogInformation("Sending POST:[{Url}]", postUrl);
        using var request = await new HttpRequestMessage(HttpMethod.Post, postUrl)
            .AppendAuthorizationHeader(injectedDependencies);
        request.Content = new ByteArrayContent(selectedVictualImage);
        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }
}