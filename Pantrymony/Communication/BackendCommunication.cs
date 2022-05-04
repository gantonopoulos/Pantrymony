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
}