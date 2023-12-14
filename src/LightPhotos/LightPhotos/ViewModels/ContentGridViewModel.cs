using System.Collections.ObjectModel;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LightPhotos.Contracts.Services;
using LightPhotos.Contracts.ViewModels;
using LightPhotos.Core.Contracts.Services;
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
        Source.Clear();

        if (parameter is StorageFolder folder)
        {
            var fileList = await folder.GetFilesAsync();
            for (var i = 0; i < fileList.Count; i++)
            {
                var file = fileList[i];
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
                await bitmapImage?.SetSourceAsync(await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView));
            }
        }

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
