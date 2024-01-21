using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPhotos.Core.Helpers;
public static class ImageCodecInfoHelper
{
    private static readonly ImageCodecInfo? s_jpeg;
    private static readonly ImageCodecInfo? s_webp;
    private static readonly EncoderParameters? s_jpegEncoderParameters;
    private static readonly EncoderParameters? s_jpegEncoderFullQualityParameters;
    private static readonly EncoderParameters? s_webpEncoderParameters;
    public const int JpegQuality = 83;
    public const int WebpQuality = 75;
    public const int WebpFullQuality = 75;

    public static ImageCodecInfo? Jpeg => s_jpeg;

    //public static ImageCodecInfo? Webp => s_webp;

    public static EncoderParameters? JpegEncoderParameters => s_jpegEncoderParameters;

    public static EncoderParameters? JpegEncoderFullQualityParameters => s_jpegEncoderParameters;

    //public static EncoderParameters? WebpEncoderParameters => s_webpEncoderParameters;

    static ImageCodecInfoHelper()
    {
        s_jpeg = GetEncoderInfo("image/jpeg");
        s_webp = GetEncoderInfo("image/webp");
        s_jpegEncoderParameters = GetEncoderParameters(JpegQuality);
        s_jpegEncoderFullQualityParameters = GetEncoderParameters(100);
        s_webpEncoderParameters = GetEncoderParameters(WebpQuality);
    }

    private static ImageCodecInfo? GetEncoderInfo(string mimeType)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
        }
        return null;
    }

    private static EncoderParameters? GetEncoderParameters(long quality)
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            var encoder = System.Drawing.Imaging.Encoder.Quality;
            var encoderParameter = new EncoderParameter(encoder, quality);
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;
            return encoderParameters;
        }
        return null;
    }
}
