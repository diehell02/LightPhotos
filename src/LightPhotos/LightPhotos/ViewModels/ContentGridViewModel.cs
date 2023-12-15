using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LightPhotos.Contracts.Services;
using LightPhotos.Contracts.ViewModels;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Logging;
using LightPhotos.Core.Models;
using LightPhotos.Models;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace LightPhotos.ViewModels;

public partial class ContentGridViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    private readonly ISampleDataService _sampleDataService;

    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public ObservableCollection<Picture> BitmapImageSource { get; } = new ObservableCollection<Picture>();

    public ContentGridViewModel(INavigationService navigationService, ISampleDataService sampleDataService)
    {
        _navigationService = navigationService;
        _sampleDataService = sampleDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Log.Information("Enter");
        Source.Clear();

        if (parameter is StorageFolder folder)
        {
            var path = folder.Path;
            var directory = new DirectoryInfo(path);
            var files = directory.GetFiles();
            for (var i = 0; i < files.Length; i++)
            {
                var fileInfo = files[i];
                var filePath = fileInfo.FullName;
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                BitmapImage? bitmapImage;
                if (BitmapImageSource.Count <= i)
                {
                    bitmapImage = new BitmapImage();
                    BitmapImageSource.Add(new Picture(bitmapImage, file));
                }
                else
                {
                    BitmapImageSource[i].StorageFile = file;
                    bitmapImage = BitmapImageSource[i].ThumbnailBitmapImage;
                }
                var thumbnail = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
                Log.Information("GetThumbnailAsync Finished");
                await bitmapImage?.SetSourceAsync(thumbnail);
                Log.Information("SetSourceAsync Finished");
            }
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
}
