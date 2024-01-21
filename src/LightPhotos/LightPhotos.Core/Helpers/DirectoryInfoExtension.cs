using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightPhotos.Core.Protos;
using LightPhotos.Core.Services;

namespace LightPhotos.Core.Helpers;
public static partial class DirectoryInfoExtension
{
    public static Category ToCategory(this DirectoryInfo directoryInfo)
    {
        var category = new Category()
        {
            Id = Guid.NewGuid().ToString(),
            Name = directoryInfo.Name,
            FullName = directoryInfo.FullName
        };
        foreach (var directory in directoryInfo.GetDirectories())
        {
            category.Categories.Add(directory.ToCategory());
        }
        var regex = FileHelper.WICRegex();
        var files = directoryInfo.GetFiles().Where(file => regex.IsMatch(file.Name));
        foreach (var file in files)
        {
            category.ImageFiles.Add(file.ToImageFile());
        }
        return category;
    }
}
