using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LightPhotos.Contracts.Services;
using LightPhotos.Contracts.ViewModels;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Models;
using LightPhotos.Core.Protos;
using LightPhotos.Models;
using Microsoft.UI.Xaml.Media.Imaging;

namespace LightPhotos.ViewModels;

public partial class ContentGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;

    private readonly IPhotoService _photoService;

    //[ObservableProperty]
    //private SampleOrder? item;

    [ObservableProperty]
    private Picture? item;

    [ObservableProperty]
    private BitmapImage? bitmapImage;

    public ICommand GoBackCommand
    {
        get;
    }

    public ContentGridDetailViewModel(INavigationService navigationService, IPhotoService photoService)
    {
        _navigationService = navigationService;
        _photoService = photoService;

        GoBackCommand = new RelayCommand(OnGoBack);
    }

    public async void OnNavigatedTo(object parameter)
    {
        //if (parameter is long orderID)
        //{
        //    var data = await _sampleDataService.GetContentGridDataAsync();
        //    Item = data.First(i => i.OrderID == orderID);
        //}
        if (parameter is Picture picture)
        {
            Item = picture;
            var bitmapImage = new BitmapImage();
            var bytes = await _photoService.GetContentAsync(picture.ImageFile);
            using var memoryStream = new MemoryStream(bytes);
            await bitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
            BitmapImage = bitmapImage;
        }
    }

    public void OnNavigatedFrom()
    {
    }

    private void OnGoBack()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
    }
}
