using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using LightPhotos.Core.Protos;
using Microsoft.UI.Xaml.Media.Imaging;

namespace LightPhotos.Models;
public partial class Picture : ObservableObject
{
    public ImageFile ImageFile
    {
        get; private set;
    }

    public bool IsLoaded => ThumbnailBitmapImage != null;

    [ObservableProperty]
    private BitmapImage? thumbnailBitmapImage = null;

    public Picture(ImageFile storageFile)
    {
        ImageFile = storageFile;
    }

    public Picture(BitmapImage bitmapImage, ImageFile storageFile)
    {
        ThumbnailBitmapImage = bitmapImage;
        ImageFile = storageFile;
    }
}
