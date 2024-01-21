using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightPhotos.Core.Protos;

namespace LightPhotos.Core.Contracts.Services;
public interface IPhotoService
{
    void SetServiceAddress(string serviceAddress);

    Task<IEnumerable<Category>> GetRootCategoriesAsync();

    Task<IEnumerable<Category>> GetCategoriesAsync(Category category);

    Task<IEnumerable<ImageFile>> GetFilesAsync(Category category);

    Task<byte[]> GetThumbnailAsync(ImageFile file, uint viewPortWidth = 0, uint viewPortHeight = 0);

    Task<byte[]> GetContentAsync(ImageFile file);
}
