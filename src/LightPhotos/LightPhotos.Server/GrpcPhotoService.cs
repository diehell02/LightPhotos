using System.IO;
using LightPhotos.Core.Protos;
using LightPhotos.Core.Services;
using LightPhotos.Core.Helpers;
using Grpc.Core;
using LightPhotos.Core.Logging;
using Google.Protobuf;

namespace LightPhotos.Server;

public class GrpcPhotoService : LocalPhotoService
{
    public override Task<IEnumerable<Category>> GetRootCategoriesAsync()
    {
        if (Config.Instance is null)
        {
            return Task.FromResult(Enumerable.Empty<Category>());
        }
        return Task.FromResult(Config.Instance.RootFolder.Select(folder =>
        {
            var directoryInfo = new DirectoryInfo(folder);
            return directoryInfo.ToCategory();
        }));
    }

    public async override Task<GetRootCategoriesResponse> GetRootCategories(GetRootCategoriesRequest request, ServerCallContext context)
    {
        try
        {
            var categories = await base.GetRootCategoriesAsync();
            var response = new GetRootCategoriesResponse();
            response.Categories.AddRange(categories);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new GetRootCategoriesResponse();
        }
    }

    public async override Task<GetCategoriesResponse> GetCategories(GetCategoriesRequest request, ServerCallContext context)
    {
        try
        {
            var categories = await base.GetCategoriesAsync(request.Category);
            var response = new GetCategoriesResponse();
            response.Categories.AddRange(categories);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new GetCategoriesResponse();
        }
    }

    public async override Task<GetFilesResponse> GetFiles(GetFilesRequest request, ServerCallContext context)
    {
        try
        {
            var files = await base.GetFilesAsync(request.Category);
            var response = new GetFilesResponse();
            response.Files.AddRange(files);
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new GetFilesResponse();
        }
    }

    public async override Task<GetThumbnailResponse> GetThumbnail(GetThumbnailRequest request, ServerCallContext context)
    {
        try
        {
            var bytes = await base.GetThumbnailAsync(request.File, request.ViewPortWidth, request.ViewPortHeight);
            var response = new GetThumbnailResponse
            {
                Data = ByteString.CopyFrom(bytes)
            };
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new GetThumbnailResponse();
        }
    }

    public async override Task<GetContentResponse> GetContent(GetContentRequest request, ServerCallContext context)
    {
        try
        {
            var bytes = await base.GetContentAsync(request.File);
            var response = new GetContentResponse
            {
                Data = ByteString.CopyFrom(bytes)
            };
            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return new GetContentResponse();
        }
    }
}
