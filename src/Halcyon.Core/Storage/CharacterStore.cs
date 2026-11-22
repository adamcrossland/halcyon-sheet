using System.Text.Json;
using System.Text.Json.Serialization;
using Halcyon.Core.Model;

namespace Halcyon.Core.Storage;

/// <summary>Saves characters as one JSON file each in a folder.</summary>
public sealed class CharacterStore
{
    public static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
    };

    readonly string _dir;
    readonly Lock _gate = new();

    public CharacterStore(string directory)
    {
        _dir = directory;
        Directory.CreateDirectory(_dir);
    }

    public string Folder => _dir;

    string PathFor(Guid id) => Path.Combine(_dir, $"{id:N}.json");

    public IReadOnlyList<Character> List()
    {
        var list = new List<Character>();
        foreach (var file in Directory.EnumerateFiles(_dir, "*.json"))
        {
            try
            {
                if (JsonSerializer.Deserialize<Character>(File.ReadAllText(file), Json) is { } c) list.Add(c);
            }
            catch (JsonException) { /* skip unreadable files */ }
        }
        return list.OrderByDescending(c => c.Updated).ToList();
    }

    public Character? Get(Guid id)
    {
        var path = PathFor(id);
        return File.Exists(path) ? JsonSerializer.Deserialize<Character>(File.ReadAllText(path), Json) : null;
    }

    public void Save(Character c)
    {
        c.Updated = DateTimeOffset.UtcNow;
        var json = JsonSerializer.Serialize(c, Json);
        lock (_gate)
        {
            var path = PathFor(c.Id);
            var tmp = path + ".tmp";
            File.WriteAllText(tmp, json);
            File.Move(tmp, path, overwrite: true);
        }
    }

    public bool Delete(Guid id)
    {
        var path = PathFor(id);
        if (!File.Exists(path)) return false;
        File.Delete(path);
        return true;
    }

    public static string Export(Character c) => JsonSerializer.Serialize(c, Json);

    /// <summary>Reads an exported character. Gives it a new id if one with the same id already exists.</summary>
    public Character Import(string json)
    {
        var c = JsonSerializer.Deserialize<Character>(json, Json) ?? throw new JsonException("Not a character file.");
        if (string.IsNullOrWhiteSpace(c.Name) && string.IsNullOrWhiteSpace(c.ClassId)) throw new JsonException("Not a character file.");
        if (c.Id == Guid.Empty || Get(c.Id) is not null) c.Id = Guid.NewGuid();
        Save(c);
        return c;
    }
}
