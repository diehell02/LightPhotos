using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightPhotos.Contracts.Services;
using Windows.Foundation;
using Windows.Storage;

namespace LightPhotos.Services;
internal class StoragePickerService : IStoragePickerService
{
    public IAsyncOperation<StorageFolder> PickSingleFolderAsync()
    {
        var folderPicker = new Windows.Storage.Pickers.FolderPicker
        {
            SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
        };
        // Get the current window's HWND by passing in the Window object
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);

        // Associate the HWND with the file picker
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);
        folderPicker.FileTypeFilter.Add("*");
        return folderPicker.PickSingleFolderAsync();
    }
}
