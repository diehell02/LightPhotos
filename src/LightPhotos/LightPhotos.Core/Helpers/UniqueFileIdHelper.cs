using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LightPhotos.Core.Helpers;
public static class UniqueFileIdHelper
{
    public static string GetUniqueFileID(string filePath)
    {
        //using var fileStream = new FileStream(filePath, FileMode.Open);
        //var fileBytes = new byte[fileStream.Length];
        //fileStream.Read(fileBytes, 0, fileBytes.Length);
        //var hashBytes = SHA1.HashData(fileBytes);
        //var hashString = BitConverter.ToString(hashBytes);
        //hashString = hashString.Replace("-", "").ToLowerInvariant();
        //return hashString;

        var hashBytes = SHA1.HashData(Encoding.UTF8.GetBytes(filePath));
        var hashString = BitConverter.ToString(hashBytes);
        hashString = hashString.Replace("-", "").ToLowerInvariant();
        return hashString;
    }
}
