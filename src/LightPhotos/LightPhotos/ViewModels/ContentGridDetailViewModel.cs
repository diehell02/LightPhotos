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

public partial class ContentGridDetailViewModel : ObservableRecipient, INavigationAware
{
    private readonly ISampleDataService _sampleDataService;

    private readonly INavigationService _navigationService;

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

    public ContentGridDetailViewModel(ISampleDataService sampleDataService, INavigationService navigationService)
    {
        _sampleDataService = sampleDataService;
        _navigationService = navigationService;

        GoBackCommand = new RelayCommand(OnGoBack);
    }

    public void OnNavigatedTo(object parameter)
    {
        //if (parameter is long orderID)
        //{
        //    var data = await _sampleDataService.GetContentGridDataAsync();
        //    Item = data.First(i => i.OrderID == orderID);
        //}
        if (parameter is Picture picture)
        {
            Item = picture;
            BitmapImage = new BitmapImage(new Uri(picture.StorageFile.Path, UriKind.RelativeOrAbsolute));
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
