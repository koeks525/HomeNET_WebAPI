using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;

namespace HomeNetAPI.Services
{
    public interface IImageProcessor
    {
        String ResizeImage(SKBitmap bitmap, String fileLocation, String fileName);
    }
}
