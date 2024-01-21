using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LightPhotos.Contracts.Services;
using LightPhotos.Contracts.ViewModels;
using LightPhotos.Core.Contracts.Services;
using LightPhotos.Core.Protos;

namespace LightPhotos.ViewModels;

public partial class MainViewModel(INavigationService _navigationService) : ObservableRecipient, INavigationAware
{
    [ObservableProperty]
    private ObservableCollection<Category> _categories = [];

    public void OnNavigatedFrom()
    {
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is IEnumerable<Category> categories)
        {
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }
    }

    [RelayCommand]
    private void ItemClick(Category? clickedItem)
    {
        _navigationService.NavigateTo(typeof(ContentGridViewModel).FullName!, clickedItem);
    }
}
