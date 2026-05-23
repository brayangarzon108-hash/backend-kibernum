using System.Net.Http.Json;
using IntergalaxyTech.Application.Abstractions;

namespace IntergalaxyTech.Infrastructure.External;

public class RickAndMortyClient(HttpClient httpClient) : IRickAndMortyClient
{
    public async Task<RickAndMortyCharacterResponse> GetCharactersAsync(int page, string? name, CancellationToken ct)
    {
        var url = $"character?page={page}" + (string.IsNullOrWhiteSpace(name) ? string.Empty : $"&name={Uri.EscapeDataString(name)}");
        var data = await httpClient.GetFromJsonAsync<RickAndMortyCharacterResponse>(url, ct);
        return data ?? new RickAndMortyCharacterResponse(new RickAndMortyInfo(0, 0, null, null), []);
    }
}
