using System.Text;

using LightPhotos.Core.Contracts.Services;

using Newtonsoft.Json;

namespace LightPhotos.Core.Services;

public class FileService : IFileService
{
    private readonly object _lock = new();

    public T Read<T>(string folderPath, string fileName)
    {
        var path = Path.Combine(folderPath, fileName);
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        return default;
    }

    public void Save<T>(string folderPath, string fileName, T content)
    {
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        var fileContent = JsonConvert.SerializeObject(content);
        File.WriteAllText(Path.Combine(folderPath, fileName), fileContent, Encoding.UTF8);
    }

    public void Delete(string folderPath, string fileName)
    {
        if (fileName != null && File.Exists(Path.Combine(folderPath, fileName)))
        {
            File.Delete(Path.Combine(folderPath, fileName));
        }
    }

    public bool Exists(string path)
    {
        lock(_lock) 
        {
            if (File.Exists(path)) 
            {
                return true;
            } 
            else if (Directory.Exists(path)) 
            {
                return true;
            } 
            else
            {
                return false;
            }
        }
    }

    public byte[] ReadAllBytes(string filePath)
    {
        lock(_lock) 
        {
            return File.ReadAllBytes(filePath);
        }
    }

    public void WriteAllBytes(string filePath, byte[] bytes)
    {
        lock(_lock)
        {
            if (File.Exists(filePath)) 
            {
                return;
            }
            else
            {
                File.WriteAllBytes(filePath, bytes);
            } 
        }
    }
}
