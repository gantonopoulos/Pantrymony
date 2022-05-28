using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Components.Forms;
using Pantrymony.Auth.Extensions;
using Pantrymony.Common;
using Pantrymony.Extensions;
using Pantrymony.Model;

namespace Pantrymony.Communication;

internal static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(Victual editedEntry, PageInjectedDependencies injectedDependencies)
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
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadFromJsonAsync<List<Unit>>();
                var result = responseData != null
                    ? responseData.OrderBy(unit => unit.Symbol).ToList()
                    : new List<Unit>();
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
    
    public static async Task<Victual> FetchVictualOfUserAsync(
        string userId, 
        string victualId, 
        PageInjectedDependencies injectedDependencies)
    {
        try
        {
            var getUrl =
                $"{injectedDependencies.Configuration["TargetApi"]}/uservictual?userId={userId}&victualId={victualId}";
            var entries = await FetchVictualsAsync(getUrl, injectedDependencies);
            
            var result = entries.Single();
            result.ImageUrl = await GetDownloadUrlAsync(result, injectedDependencies);
            return result;
        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("{Message}:\n{Stack}", e.Message, e.StackTrace);
            throw;
        }
    }
    
    public static async Task<IEnumerable<Victual>> FetchVictualsOfUserAsync(
        string userId,  
        PageInjectedDependencies injectedDependencies)
    {
        try
        {
            var getUrl = $"{injectedDependencies.Configuration["TargetApi"]}/uservictuals?userId={userId}";
            var entries = await FetchVictualsAsync(getUrl, injectedDependencies);
            return entries;

        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("Victuals could not be parsed!\n{Message}{Stack}", 
                e.Message, 
                e.StackTrace);
            throw;
        }
    }

    private static async Task<List<Victual>> FetchVictualsAsync(string getUrl, PageInjectedDependencies injectedDependencies)
    {
        try
        {
            injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);
            injectedDependencies.Logger.LogInformation("Fetching Victuals!");

            var requestMsg = await new HttpRequestMessage(HttpMethod.Get, getUrl)
                .AppendAuthorizationHeader(injectedDependencies);
            var response = await injectedDependencies.HttpClient.SendAsync(requestMsg);
            
            if (!response.IsSuccessStatusCode) return new List<Victual>();
            
            var result = await response.Content.ReadFromJsonAsync<List<Victual>>();
            result = result.ThrowIfNull(new Exception("Error while deserializing received victuals."));
            return result;
        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("Victuals could not be parsed!\n{Message}{Stack}", 
                e.Message, 
                e.StackTrace);
            return new List<Victual>();
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
        injectedDependencies.Logger.LogInformation("Adding Entry with id {Identifier}",
            editedEntry.VictualId.ToString());

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
        IBrowserFile metadataOfUploadImage, 
        PageInjectedDependencies injectedDependencies)
    {
        
        var uploadUrl = await GetUploadUrlAsync(imageOwner, injectedDependencies);
        await UploadImage(uploadUrl, metadataOfUploadImage, injectedDependencies);
    }
    
    private static async Task UploadImage(string uploadUrl, IBrowserFile metadataOfUploadImage,
        PageInjectedDependencies injectedDependencies)
    {
        injectedDependencies.Logger.LogInformation("Sending PUT:[{@UploadUrl}]", uploadUrl);
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, uploadUrl))
        {
            request.Content = new StreamContent(metadataOfUploadImage.OpenReadStream());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(metadataOfUploadImage.ContentType);
            
            injectedDependencies.Logger.LogInformation("Pre request Header:{Header}", JsonSerializer.Serialize(request));
            using (var response = await injectedDependencies.HttpClient
                       .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                       .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }
        }
    }
    
    public static async Task DeleteImageAsync(Victual imageOwner, PageInjectedDependencies injectedDependencies)
    {
        var deleteUrl = await GetDeleteUrlAsync(imageOwner, injectedDependencies);
        await DeleteImage(deleteUrl, injectedDependencies);
    }

    private static async Task DeleteImage(string uploadUrl, PageInjectedDependencies injectedDependencies)
    {
        injectedDependencies.Logger.LogInformation("Sending DELETE:[{@UploadUrl}]", uploadUrl);
        using var request = new HttpRequestMessage(HttpMethod.Delete, uploadUrl);
        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }


    private static async Task<string> GetUploadUrlAsync(Victual imageOwner, PageInjectedDependencies injectedDependencies)
    {
         var getUrl = $"{injectedDependencies.Configuration["TargetApi"]}/uploadImageUrl?imageKey={imageOwner.VictualId}";
        injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);
        using var request = await new HttpRequestMessage(HttpMethod.Get, getUrl)
            .AppendAuthorizationHeader(injectedDependencies);
        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<string>( await response.Content.ReadAsByteArrayAsync())
            .ThrowIfNull(new Exception("Unexpected content format"));
    }
    
    private static async Task<string> GetDownloadUrlAsync(Victual imageOwner, PageInjectedDependencies injectedDependencies)
    {
        try
        {
            var getUrl = $"{injectedDependencies.Configuration["TargetApi"]}/downloadImageUrl?imageKey={imageOwner.VictualId}";
            injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);
            using var request = await new HttpRequestMessage(HttpMethod.Get, getUrl)
                .AppendAuthorizationHeader(injectedDependencies);
            using var response = await injectedDependencies.HttpClient
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);
            {
                response.ThrowIf(r => !r.IsSuccessStatusCode && r.StatusCode != HttpStatusCode.NotFound,
                    new Exception($"Unexpected status code: {response.StatusCode}"));

                if (!response.IsSuccessStatusCode)
                    return string.Empty;
                return JsonSerializer.Deserialize<string>( await response.Content.ReadAsByteArrayAsync())
                    .ThrowIfNull(new Exception("Unexpected content format"));
            }
        }
        catch (Exception e)
        {
            injectedDependencies.Logger.LogError("Download url could not be retrieved\n{Message}{Stack}",
                e.Message, e.StackTrace);
            throw;
        }
    }
    
    private static async Task<string> GetDeleteUrlAsync(Victual imageOwner, PageInjectedDependencies injectedDependencies)
    {
        var getUrl = $"{injectedDependencies.Configuration["TargetApi"]}/deleteImageUrl?imageKey={imageOwner.VictualId}";
        injectedDependencies.Logger.LogInformation("Sending GET:[{Url}]", getUrl);
        using var request = await new HttpRequestMessage(HttpMethod.Get, getUrl)
            .AppendAuthorizationHeader(injectedDependencies);
        using var response = await injectedDependencies.HttpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<string>( await response.Content.ReadAsByteArrayAsync())
            .ThrowIfNull(new Exception("Unexpected content format"));
    }
}