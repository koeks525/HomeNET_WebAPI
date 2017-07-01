using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using System.IO;

namespace HomeNetAPI.Services
{
    public class ImageProcessor : IImageProcessor
    {
        //Source: https://gist.github.com/xoofx/a9d08a37c43f692e65df80a1888c488b 

        public String ResizeImage(SKBitmap bitmap, String fileLocation, String fileName)
        {
            
            float resizeFactor = 0.5f;
            var newBitmap = new SKBitmap((int)Math.Round(bitmap.Width * resizeFactor), (int)Math.Round(bitmap.Height * resizeFactor));
            var canvas = new SKCanvas(newBitmap);
            canvas.SetMatrix(SKMatrix.MakeScale(resizeFactor, resizeFactor));
            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.ResetMatrix();
            canvas.Flush();
            var resultImage = SKImage.FromBitmap(newBitmap);
            var resultImageData = resultImage.Encode(SKEncodedImageFormat.Jpeg, 100);
            String finalLocation = fileLocation + "optimized_" + fileName;
            using (var stream = new FileStream(fileLocation, FileMode.Create, FileAccess.ReadWrite))
            {
                resultImageData.SaveTo(stream);
                resultImageData.Dispose();
                resultImage.Dispose();
                canvas.Dispose();
                newBitmap.Dispose();
                bitmap.Dispose();
                return finalLocation;
                
            }
        }
    }
}
