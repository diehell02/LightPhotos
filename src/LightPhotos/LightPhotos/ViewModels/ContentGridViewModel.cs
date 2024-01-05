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
using LightPhotos.Models;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.UI.Core;

namespace LightPhotos.ViewModels;

public partial class ContentGridViewModel : ObservableRecipient, INavigationAware
{
    private class ItemProvider : IItemsProvider<Picture>
    {
        FileInfo[]? _fileInfos = null;

        public int Count => _fileInfos?.Length ?? 0;

        public void SetFiles(FileInfo[] fileInfos)
        {
            _fileInfos = fileInfos;
        }

        public async Task<IList<Picture>> Fetch(int startIndex, uint count)
        {
            if (_fileInfos is null)
            {
                return null;
            }
            var result = new List<Picture>();
            for (var i = startIndex; i < _fileInfos.Length; i++)
            {
                var fileInfo = _fileInfos[i];
                var filePath = fileInfo.FullName;
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                var bitmapImage = new BitmapImage();
                var picture = new Picture(bitmapImage, file);
                var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
                await bitmapImage?.SetSourceAsync(thumbnail);
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
            var filePath = fileInfo.FullName;
            var picture = new Picture(filePath);            
            return picture;
        }

        public async void LoadData(Picture picture)
        {
            if (picture.IsLoaded)
            {
                return;
            }
            picture.IsLoaded = true;
            //DispatcherQueue.GetForCurrentThread().TryEnqueue(async () =>
            //{
            //    var filePath = picture.Path;
            //    var file = await StorageFile.GetFileFromPathAsync(filePath);
            //    var bitmapImage = new BitmapImage();
            //    var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            //    await bitmapImage?.SetSourceAsync(thumbnail);
            //    picture.StorageFile = file;
            //    picture.ThumbnailBitmapImage = bitmapImage;
            //});
            var filePath = picture.Path;
            var file = await StorageFile.GetFileFromPathAsync(filePath);
            var bitmapImage = new BitmapImage();
            var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            await bitmapImage?.SetSourceAsync(thumbnail);
            picture.StorageFile = file;
            picture.ThumbnailBitmapImage = bitmapImage;
        }
    }

    private readonly INavigationService _navigationService;
    private readonly ISampleDataService _sampleDataService;
    private readonly ItemProvider _itemProvider;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public DataVirtualizationCollection<Picture> BitmapImageSource { get; private set; }

    public ContentGridViewModel(INavigationService navigationService, ISampleDataService sampleDataService)
    {
        _navigationService = navigationService;
        _sampleDataService = sampleDataService;
        _itemProvider = new ItemProvider();
    }

    public void OnNavigatedTo(object parameter)
    {
        Log.Information("Enter");
        Source.Clear();

        if (parameter is StorageFolder folder)
        {
            var path = folder.Path;
            var directory = new DirectoryInfo(path);
            Regex regex = WICRegex();
            var files = directory.GetFiles().Where(file => regex.IsMatch(file.Name));
            _itemProvider.SetFiles(files.ToArray());
            BitmapImageSource = new DataVirtualizationCollection<Picture>(_itemProvider);
            //for (var i = 0; i < files.Length; i++)
            //{
            //    var fileInfo = files[i];
            //    var filePath = fileInfo.FullName;
            //    var file = await StorageFile.GetFileFromPathAsync(filePath);
            //    BitmapImage? bitmapImage;
            //    if (BitmapImageSource.Count <= i)
            //    {
            //        bitmapImage = new BitmapImage();
            //        BitmapImageSource.Add(new Picture(bitmapImage, file));
            //    }
            //    else
            //    {
            //        BitmapImageSource[i].StorageFile = file;
            //        bitmapImage = BitmapImageSource[i].ThumbnailBitmapImage;
            //    }
            //    var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            //    Log.Information("GetThumbnailAsync Finished");
            //    await bitmapImage?.SetSourceAsync(thumbnail);
            //    Log.Information("SetSourceAsync Finished");
            //}
        }

        Log.Information("Exit");

        //// TODO: Replace with real data.
        //var data = await _sampleDataService.GetContentGridDataAsync();
        //foreach (var item in data)
        //{
        //    Source.Add(item);
        //}
    }

    public void OnNavigatedFrom()
    {
    }

    [RelayCommand]
    //private void OnItemClick(SampleOrder? clickedItem)
    //{
    //    if (clickedItem != null)
    //    {
    //        _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
    //        _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName!, clickedItem.OrderID);
    //    }
    //}
    private void OnItemClick(Picture? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ContentGridDetailViewModel).FullName!, clickedItem);
        }
    }

    [GeneratedRegex(@"^(?i).+\.(jpe|jpeg|jpg|png|bmp|dib|gif|tiff|tif|jxr|wdp|ico)$")]
    private static partial Regex WICRegex();
}
