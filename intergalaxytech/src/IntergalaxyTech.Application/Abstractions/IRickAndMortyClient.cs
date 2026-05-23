namespace IntergalaxyTech.Application.Abstractions;

public interface IRickAndMortyClient
{
    Task<RickAndMortyCharacterResponse> GetCharactersAsync(int page, string? name, CancellationToken ct);
}

public record RickAndMortyCharacterResponse(RickAndMortyInfo Info, List<RickAndMortyCharacter> Results);
public record RickAndMortyInfo(int Count, int Pages, string? Next, string? Prev);
public record RickAndMortyCharacter(int Id, string Name, string Status, string Species, string Gender, RickAndMortyPlace Origin, RickAndMortyPlace Location, string Image);
public record RickAndMortyPlace(string Name);
