using System.Net.Http.Json;
using Pantrymony.Model;

namespace Pantrymony.Communication;

public static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(HttpClient client, IConfiguration configuration, Victual editedEntry)
    {
        Console.WriteLine($"Updating Entry with id {editedEntry.Identifier}");
        HttpResponseMessage response = await client.PutAsJsonAsync(
            configuration["TargetApi"] + $"/victuals/{editedEntry.Identifier}", editedEntry);

        Console.WriteLine(response.StatusCode);
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}