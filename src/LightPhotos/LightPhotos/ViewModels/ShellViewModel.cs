using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LightPhotos.Contracts.Services;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace LightPhotos.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;

    public ICommand MenuFileExitCommand
    {
        get;
    }

    public ICommand MenuSettingsCommand
    {
        get;
    }

    public ICommand MenuViewsContentGridCommand
    {
        get;
    }

    public ICommand MenuViewsListDetailsCommand
    {
        get;
    }

    public ICommand MenuViewsMainCommand
    {
        get;
    }

    public INavigationService NavigationService
    {
        get;
    }

    public ICommand MenuFileOpenCommand
    {
        get;
    }

    public IStoragePickerService StoragePickerService
    {
        get;
    }

    public ShellViewModel(INavigationService navigationService, IStoragePickerService storagePickerService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        StoragePickerService = storagePickerService;

        MenuFileExitCommand = new RelayCommand(OnMenuFileExit);
        MenuFileOpenCommand = new RelayCommand(OnMenuFileOpen);
        MenuSettingsCommand = new RelayCommand(OnMenuSettings);
        MenuViewsContentGridCommand = new RelayCommand(OnMenuViewsContentGrid);
        MenuViewsListDetailsCommand = new RelayCommand(OnMenuViewsListDetails);
        MenuViewsMainCommand = new RelayCommand(OnMenuViewsMain);
    }

    private void OnNavigated(object sender, NavigationEventArgs e) => IsBackEnabled = NavigationService.CanGoBack;

    private void OnMenuFileExit() => Application.Current.Exit();

    private void OnMenuSettings() => NavigationService.NavigateTo(typeof(SettingsViewModel).FullName!);

    private void OnMenuViewsContentGrid() => NavigationService.NavigateTo(typeof(ContentGridViewModel).FullName!);

    private void OnMenuViewsListDetails() => NavigationService.NavigateTo(typeof(ListDetailsViewModel).FullName!);

    private void OnMenuViewsMain() => NavigationService.NavigateTo(typeof(MainViewModel).FullName!);

    private async void OnMenuFileOpen()
    {
        var folder = await StoragePickerService.PickSingleFolderAsync();
        if (folder is null)
        {
            return;
        }
        NavigationService.NavigateTo(typeof(ContentGridViewModel).FullName!, folder);
    }
}
