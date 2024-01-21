using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LightPhotos.Core.Helpers;
public partial class FileHelper
{
    //private const string APP_NAME = "LightPhotos";
    public static readonly string ImageFolder;
    public static readonly string ThumbnailFolder;

    static FileHelper()
    {
        //var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        //var currentLocalAppData = Path.Combine(localApplicationData, APP_NAME);
        //if (!Directory.Exists(currentLocalAppData))
        //{
        //    Directory.CreateDirectory(currentLocalAppData);
        //}
        ImageFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
        ThumbnailFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Thumbnails");

        if (!Directory.Exists(ImageFolder))
        {
            Directory.CreateDirectory(ImageFolder);
        }
        if (!Directory.Exists(ThumbnailFolder))
        {
            Directory.CreateDirectory(ThumbnailFolder);
        }
    }

    [GeneratedRegex(@"^(?i).+\.(jpe|jpeg|jpg|png|bmp|dib|gif|tiff|tif|jxr|wdp|ico)$")]
    public static partial Regex WICRegex();
}
