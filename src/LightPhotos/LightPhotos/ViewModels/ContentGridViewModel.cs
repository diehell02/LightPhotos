using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightPhotos.Collections;
using LightPhotos.Contracts.Services;
using LightPhotos.Contracts.ViewModels;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Logging;
using LightPhotos.Core.Models;
using LightPhotos.Core.Protos;
using LightPhotos.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI.Core;

namespace LightPhotos.ViewModels;

public partial class ContentGridViewModel : ObservableRecipient, INavigationAware
{
    private class ItemProvider(IPhotoService _photoService) : IItemsProvider<Picture>
    {
        ImageFile[]? _fileInfos = null;

        public int Count => _fileInfos?.Length ?? 0;

        public void SetFiles(ImageFile[] fileInfos)
        {
            _fileInfos = fileInfos;
        }

        public async Task<IList<Picture>> Fetch(int startIndex, uint count)
        {
            if (_fileInfos is null)
            {
                return new List<Picture>();
            }
            var result = new List<Picture>();
            for (var i = startIndex; i < _fileInfos.Length; i++)
            {
                var fileInfo = _fileInfos[i];
                var bitmapImage = new BitmapImage();
                var picture = new Picture(bitmapImage, fileInfo);
                var thumbnailBytes = await _photoService.GetThumbnailAsync(fileInfo);
                using var stream = new MemoryStream(thumbnailBytes);
                await bitmapImage?.SetSourceAsync(stream.AsRandomAccessStream());
                result.Add(picture);
            }
            return result;
        }

        public Picture Fetch(int index)
        {
            if (_fileInfos is null)
            {
                return null;
            }
            var fileInfo = _fileInfos[index];
            var picture = new Picture(fileInfo);            
            return picture;
        }

        public async void LoadData(Picture picture)
        {
            if (picture.IsLoaded)
            {
                return;
            }
            var bitmapImage = new BitmapImage();
            var thumbnailBytes = await _photoService.GetThumbnailAsync(picture.ImageFile);
            using var stream = new MemoryStream(thumbnailBytes);
            await bitmapImage?.SetSourceAsync(stream.AsRandomAccessStream());
            picture.ThumbnailBitmapImage = bitmapImage;
        }
    }

    private readonly INavigationService _navigationService;
    private readonly IPhotoService _photoService;
    private readonly ItemProvider _itemProvider;

    [ObservableProperty]
    private DataVirtualizationCollection<Picture>? _bitmapImageSource;

    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    public ContentGridViewModel(INavigationService navigationService, IPhotoService photoService)
    {
        _navigationService = navigationService;
        _photoService = photoService;
        _itemProvider = new ItemProvider(_photoService);
    }

    public void OnNavigatedTo(object parameter)
    {
        Log.Information("Enter");
        if (parameter is Category category)
        {
            _itemProvider.SetFiles([.. category.ImageFiles]);
            BitmapImageSource = new DataVirtualizationCollection<Picture>(_itemProvider);            
        }
        else if (parameter is IEnumerable<Category> categories)
        {
            foreach (var item in categories)
            {
                Categories.Add(item);
            }
        }
        Log.Information("Exit");
    }

    public void OnNavigatedFrom()
    {
    }

    [RelayCommand]
    private void OnFolderClick(Category? clickedItem)
    {
        _navigationService.NavigateTo(typeof(ContentGridViewModel).FullName!, clickedItem);
    }

    [RelayCommand]
    private void OnItemClick(Picture? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName!, clickedItem);
        }
    }
}
