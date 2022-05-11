using System.Net.Http.Json;
using System.Text.Json;
using Pantrymony.Model;

namespace Pantrymony.Communication;

public static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(
        HttpClient client, 
        IConfiguration configuration, 
        Victual editedEntry)
    {
        string updateUrl =
            $"{configuration["TargetApi"]}/updatevictual?userId={editedEntry.UserId}&victualId={editedEntry.VictualId}"; 
        Console.WriteLine($"Sending PUT:[{updateUrl}]");
        Console.WriteLine($"Sending content: {JsonSerializer.Serialize(editedEntry)}");
        HttpResponseMessage response = await client.PutAsJsonAsync(updateUrl, editedEntry);
        
        Console.WriteLine($"Response Code: [{response.StatusCode}]");
        Console.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");
    }
    
    public static async Task<IEnumerable<Unit>> FetchUnitsAsync(HttpClient client, IConfiguration configuration)
    {
        try
        {
            var getUrl = $"{configuration["TargetApi"]}/units";
            Console.WriteLine("Fetching Units!");
            Console.WriteLine($"Sending GET:[{getUrl}]");
            var response = await client.GetFromJsonAsync<List<Unit>>(getUrl);
            List<Unit> result = response != null? response.OrderBy(unit=>unit.Symbol).ToList(): new List<Unit>();
            result.ForEach(Console.WriteLine);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine("Units could not be parsed");
            Console.WriteLine(e);
            throw;
        }
    }
}