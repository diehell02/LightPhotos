using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightPhotos.gRPC;

public class Config
{
    public static readonly Config? Instance;

    static Config()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        var json = File.ReadAllText(path);
        Instance = JsonSerializer.Deserialize<Config>(json);
    }

    [JsonPropertyName("rootFolder")]
    public List<string> RootFolder
    {
        get; set;
    } = [];

    public Config()
    {
    }

    public Config(List<string> rootFolder)
    {
        RootFolder = rootFolder;
    }
}
