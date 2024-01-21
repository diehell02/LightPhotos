using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightPhotos.Server;

public class Config
{
    public static readonly Config? Instance;

    static Config()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        Instance = JsonSerializer.Deserialize<Config>(path);
    }

    [JsonPropertyName("rootFolder")]
    public string[] RootFolder
    {
        get; set;
    } = [];

    public Config()
    {
    }

    public Config(string[] rootFolder)
    {
        RootFolder = rootFolder;
    }
}
