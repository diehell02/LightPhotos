using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Helpers;
using LightPhotos.Core.Protos;
using System.Drawing;
using System.Drawing.Imaging;
using SkiaSharp;

namespace LightPhotos.Core.Services;
public class LocalPhotoService(IFileService _fileService) : PhotoService.PhotoServiceBase, IPhotoService
{
    private const int THUMBNAIL_SIZE = 120;

    public Task<IEnumerable<Category>> GetCategoriesAsync(Category category)
    {
        var directoryInfo = new DirectoryInfo(category.FullName);
        return Task.FromResult(directoryInfo.GetDirectories().Select(directory => directory.ToCategory()));
    }

    public Task<byte[]> GetContentAsync(ImageFile file)
    {
        var filePath = $"{FileHelper.ImageFolder}/{file.Id}";
        if (_fileService.Exists(filePath))
        {
            return Task.FromResult(_fileService.ReadAllBytes(filePath));
        }
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var image = Image.FromFile(file.FullName);
            var imageCodecInfo = ImageCodecInfoHelper.Jpeg;
            if (imageCodecInfo is null)
            {
                return Task.FromResult(Array.Empty<byte>());
            }
            using var stream = new MemoryStream();
            image.Save(stream, imageCodecInfo, ImageCodecInfoHelper.JpegEncoderFullQualityParameters);
            var bytes = stream.ToArray();
            _fileService.WriteAllBytes(filePath, bytes);
            return Task.FromResult(bytes);
        }
        else
        {
            using var sourceImageStream = File.OpenRead(file.FullName);
            var image = SKBitmap.Decode(sourceImageStream);
            using var stream = new MemoryStream();
            var skData = image.Encode(SKEncodedImageFormat.Webp, ImageCodecInfoHelper.WebpFullQuality);
            skData.SaveTo(stream);
            var bytes = stream.ToArray();
            _fileService.WriteAllBytes(filePath, bytes);
            return Task.FromResult(bytes);
        }
    }

    public Task<IEnumerable<ImageFile>> GetFilesAsync(Category category)
    {
        var directoryInfo = new DirectoryInfo(category.FullName);
        var regex = FileHelper.WICRegex();
        var files = directoryInfo.GetFiles().Where(file => regex.IsMatch(file.Name));
        return Task.FromResult(files.Select(file => file.ToImageFile()));
    }

    public virtual Task<IEnumerable<Category>> GetRootCategoriesAsync() => throw new NotImplementedException();

    public Task<byte[]> GetThumbnailAsync(ImageFile file, uint viewPortWidth = 0, uint viewPortHeight = 0)
    {
        string filePath;
        if (viewPortWidth == 0 || viewPortHeight == 0)
        {
            filePath = Path.Combine(FileHelper.ThumbnailFolder, file.Id);
        }
        else
        {
            filePath = Path.Combine(FileHelper.ThumbnailFolder, $"{file.Id}_{viewPortWidth}x{viewPortHeight}");
        }
        if (_fileService.Exists(filePath))
        {
            return Task.FromResult(_fileService.ReadAllBytes(filePath));
        }
        static void GetThumbnailSize(int imageWidth, int imageHeight, uint viewPortWidth, uint viewPortHeight, out int width, out int height)
        {
            if (viewPortWidth == 0 || viewPortHeight == 0)
            {
                width = THUMBNAIL_SIZE;
                height = (int)((decimal)imageHeight / imageWidth * width);
            }
            else
            {
                var imageRatio = (decimal)imageWidth / imageHeight;
                var viewPortRatio = (decimal)viewPortWidth / viewPortHeight;
                if (imageRatio > viewPortRatio)
                {
                    height = (int)viewPortHeight;
                    width = (int)((decimal)imageWidth / imageHeight / height);
                }
                else
                {
                    width = (int)viewPortWidth;
                    height = (int)((decimal)imageHeight / imageWidth * width);
                }
            }
        }
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var image = Image.FromFile(file.FullName);
            GetThumbnailSize(image.Width, image.Height, viewPortWidth, viewPortHeight, out var thumbnailWidth, out var thumbnailHeight);
            var thumbnail = image.GetThumbnailImage(thumbnailWidth, thumbnailHeight, () => false, IntPtr.Zero);
            using var stream = new MemoryStream();
            var imageCodecInfo = ImageCodecInfoHelper.Jpeg;
            if (imageCodecInfo is null)
            {
                return Task.FromResult(Array.Empty<byte>());
            }
            thumbnail.Save(stream, imageCodecInfo, ImageCodecInfoHelper.JpegEncoderParameters);
            var bytes = stream.ToArray();
            _fileService.WriteAllBytes(filePath, bytes);
            return Task.FromResult(bytes);
        }
        else
        {
            using var sourceImageStream = File.OpenRead(file.FullName);
            var image = SKBitmap.Decode(sourceImageStream);
            GetThumbnailSize(image.Width, image.Height, viewPortWidth, viewPortHeight, out var thumbnailWidth, out var thumbnailHeight);
            var thumbnail = image.Resize(new SKImageInfo(thumbnailWidth, thumbnailHeight), SKFilterQuality.High);
            using var stream = new MemoryStream();
            var skData = thumbnail.Encode(SKEncodedImageFormat.Webp, ImageCodecInfoHelper.WebpQuality);
            skData.SaveTo(stream);
            var bytes = stream.ToArray();
            _fileService.WriteAllBytes(filePath, bytes);
            return Task.FromResult(bytes);
        }
    }

    public void SetServiceAddress(string serviceAddress) => throw new NotImplementedException();
}