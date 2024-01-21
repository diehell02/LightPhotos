namespace LightPhotos.Core.Contracts.Services;

public interface IFileService
{
    T Read<T>(string folderPath, string fileName);

    void Save<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);

    bool Exists(string path);

    byte[] ReadAllBytes(string filePath);

    void WriteAllBytes(string filePath, byte[] bytes);
}
