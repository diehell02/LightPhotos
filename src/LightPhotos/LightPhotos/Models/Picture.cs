using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;

namespace LightPhotos.Models;
public class Picture
{
    public BitmapImage ThumbnailBitmapImage
    {
        get; set;
    }

    public StorageFile StorageFile
    {
        get; set;
    }

    public Picture(BitmapImage bitmapImage, StorageFile storageFile)
    {
        ThumbnailBitmapImage = bitmapImage;
        StorageFile = storageFile;
    }
}
