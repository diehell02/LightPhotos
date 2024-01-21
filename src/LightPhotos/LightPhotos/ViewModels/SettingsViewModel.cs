using System.Reflection;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LightPhotos.Contracts.Services;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Protos;
using LightPhotos.Helpers;
using LightPhotos.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.UI.Popups;

namespace LightPhotos.ViewModels;

public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private readonly INavigationService _navigationService;
    private readonly IPhotoService _photoService;

    [ObservableProperty]
    private ElementTheme _elementTheme;

    [ObservableProperty]
    private string _versionDescription;

    [ObservableProperty]
    private string _serverAddress;

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public ICommand GoBackCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, INavigationService navigationService, IPhotoService photoService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();
        _navigationService = navigationService;
        _photoService = photoService;

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
        GoBackCommand = new RelayCommand(OnGoBack);
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    private void OnGoBack()
    {
        if (_navigationService.CanGoBack)
        {
            _navigationService.GoBack();
        }
    }

    [RelayCommand]
    private async void SaveServerAddress()
    {
        ContentDialog contentDialog;
        if (string.IsNullOrWhiteSpace(ServerAddress))
        {
            contentDialog = new ContentDialog
            {
                Title = "Error",
                Content = "Server Address should not be empty.",
                CloseButtonText = "Ok",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };
            await contentDialog.ShowAsync();
        }
        else
        {
            _photoService.SetServiceAddress(ServerAddress);
            var rootCategories = await _photoService.GetRootCategoriesAsync();
            if (rootCategories is null)
            {
                contentDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = "Server connection failed.",
                    CloseButtonText = "Ok",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await contentDialog.ShowAsync();
            }
            else
            {
                contentDialog = new ContentDialog
                {
                    Title = "Save server address succeed",
                    Content = "You have saved the server address.",
                    CloseButtonText = "Ok",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                await contentDialog.ShowAsync();
                _navigationService.NavigateTo(typeof(ContentGridViewModel).FullName!, rootCategories);
            }
        }
    }
}
