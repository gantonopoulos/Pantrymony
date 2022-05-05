using System.Net.Http.Json;
using Pantrymony.Common;
using Pantrymony.Model;

namespace Pantrymony.Communication;

public static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(HttpClient client, IConfiguration configuration, Victual editedEntry)
    {
        Console.WriteLine($"Updating Entry {editedEntry.Identifier} of {editedEntry.UserId}");
        
        HttpResponseMessage response = await client.PutAsJsonAsync(
            configuration["TargetApi"] + $"/victuals/{editedEntry.UserId}/{editedEntry.Identifier}", editedEntry);

        Console.WriteLine($"{response.StatusCode}");
        Console.WriteLine($"{await response.Content.ReadAsStringAsync()}");
    }
    
    public static async Task<IEnumerable<Unit>> FetchUnitsAsync(HttpClient client, IConfiguration configuration)
    {
        try
        {
            Console.WriteLine("Fetching Units!");
            var response =
                await client.GetFromJsonAsync<List<Unit>>(configuration["TargetApi"] + $"/Units");
            List<Unit> result = response != null? response.OrderBy(unit=>unit.Symbol).ToList(): new List<Unit>();
            result.ForEach(Console.WriteLine);
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine("Victuals could not be parsed");
            Console.WriteLine(e);
            throw;
        }
    }
}