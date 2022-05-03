using System.Net.Http.Json;
using Pantrymony.Common;
using Pantrymony.Model;

namespace Pantrymony.Communication;

public static class BackendCommunication
{
    public static async Task SendUpdateVictualAsync(HttpClient client, IConfiguration configuration, Victual editedEntry)
    {
        Console.WriteLine("Updating Entry {VictualId} of {UserId}", editedEntry.UserId, editedEntry.Identifier);
        
        HttpResponseMessage response = await client.PutAsJsonAsync(
            configuration["TargetApi"] + $"/victuals/{editedEntry.UserId}/{editedEntry.Identifier}", editedEntry);

        Console.WriteLine("{Response}",response.StatusCode);
        Console.WriteLine("{ResponseText}", await response.Content.ReadAsStringAsync());
    }
}