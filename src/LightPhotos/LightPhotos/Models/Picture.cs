using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace LightPhotos.Models;
public partial class Picture : ObservableObject
{
    public string Path
    {
        get; private set;
    } = string.Empty;

    public bool IsLoaded
    {
        get; set;
    } = false;

    [ObservableProperty]
    private BitmapImage? thumbnailBitmapImage = null;

    [ObservableProperty]
    private StorageFile? storageFile = null;

    public Picture(string path)
    {
        Path = path;
    }

    public Picture(BitmapImage bitmapImage, StorageFile storageFile)
    {
        ThumbnailBitmapImage = bitmapImage;
        StorageFile = storageFile;
    }
}
