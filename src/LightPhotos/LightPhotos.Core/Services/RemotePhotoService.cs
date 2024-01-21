using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Helpers;
using LightPhotos.Core.Logging;
using LightPhotos.Core.Protos;

namespace LightPhotos.Core.Services;
public class RemotePhotoService : IPhotoService
{ 
    private GrpcHelper<PhotoService.PhotoServiceClient>? _grpcHelper = null;

    public async Task<IEnumerable<Category>> GetCategoriesAsync(Category category)
    {
        if (_grpcHelper is null)
        {
            return Enumerable.Empty<Category>();
        }
        var resp = await _grpcHelper.InvokeRequest<GetCategoriesRequest, GetCategoriesResponse>(
            async client => await client.GetCategoriesAsync(new GetCategoriesRequest()));
        return resp.Categories;
    }

    public async Task<byte[]> GetContentAsync(ImageFile file)
    {
        if (_grpcHelper is null)
        {
            return [];
        }
        var filePath = string.Empty;
        try
        {
            filePath = Path.Combine(FileHelper.ImageFolder, file.Id);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return [];
        }
        if (File.Exists(filePath))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            if (fileBytes.Length > 0)
            {
                return fileBytes;
            }
        }
        var resp = await _grpcHelper.InvokeRequest<GetContentRequest, GetContentResponse>(
            async client => await client.GetContentAsync(new GetContentRequest() { File = file }));
        var bytes = resp.Data.ToByteArray();
        try
        {
            File.WriteAllBytes(filePath, bytes);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return [];
        }
        return bytes;
    }

    public async Task<IEnumerable<ImageFile>> GetFilesAsync(Category category)
    {
        if (_grpcHelper is null)
        {
            return Enumerable.Empty<ImageFile>();
        }
        var resp = await _grpcHelper.InvokeRequest<GetFilesRequest, GetFilesResponse>(
            async client => await client.GetFilesAsync(new GetFilesRequest() { Category = category}));
        return resp.Files;
    }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        if (_grpcHelper is null)
        {
            return Enumerable.Empty<Category>();
        }
        var resp = await _grpcHelper.InvokeRequest<GetRootCategoriesRequest, GetRootCategoriesResponse>(
            async client => await client.GetRootCategoriesAsync(new GetRootCategoriesRequest()));
        return resp.Categories;
    }

    public async Task<byte[]> GetThumbnailAsync(ImageFile file, uint viewPortWidth = 0, uint viewPortHeight = 0)
    {
        if (_grpcHelper is null)
        {
            return [];
        }
        var filePath = string.Empty;
        try
        {
            if (viewPortWidth == 0 || viewPortHeight == 0)
            {
                filePath = Path.Combine(FileHelper.ThumbnailFolder, file.Id);
            }
            else
            {
                filePath = Path.Combine(FileHelper.ThumbnailFolder, $"{file.Id}_{viewPortWidth}x{viewPortHeight}");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return [];
        }
        if (File.Exists(filePath))
        {
            var fileBytes = File.ReadAllBytes(filePath);
            if (fileBytes.Length > 0)
            {
                return fileBytes;
            }
        }
        var resp = await _grpcHelper.InvokeRequest<GetThumbnailRequest, GetThumbnailResponse>(
            async client => await client.GetThumbnailAsync(new GetThumbnailRequest() { File = file, ViewPortWidth = viewPortWidth, ViewPortHeight = viewPortHeight }));
        var bytes = resp.Data.ToByteArray();
        try
        {
            File.WriteAllBytes(filePath, bytes);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return [];
        }
        return bytes;
    }

    public void SetServiceAddress(string serviceAddress)
    {
        if (string.IsNullOrWhiteSpace(serviceAddress))
        {
            return;
        }
        _grpcHelper = new GrpcHelper<PhotoService.PhotoServiceClient>(serviceAddress);        
    }
}
