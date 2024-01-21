using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightPhotos.Core.Protos;

namespace LightPhotos.Core.Helpers;
public static class FileInfoExtension
{
    public static ImageFile ToImageFile(this FileInfo fileInfo)
    {
        var imageFile = new ImageFile()
        {
            Id = UniqueFileIdHelper.GetUniqueFileID(fileInfo.FullName),
            Name = fileInfo.Name,
            FullName = fileInfo.FullName
        };
        return imageFile;
    }
}
