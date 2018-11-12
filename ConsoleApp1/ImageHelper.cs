using ImageMagick;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleApp1
{
    public static class ImageHelper
    {
        public static void UploadImage(byte[] byteArray, string directory, string fileName,
            int? scaleRate, bool compand = true, ResizeMode resizeMode = ResizeMode.Max, AnchorPosition anchorPosition = AnchorPosition.Center)
        {
            using (Stream uploadStream = new MemoryStream(byteArray))
            {
                using (var image = Image.Load(uploadStream))
                {
                    
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    if (scaleRate.HasValue)
                    {
                        var width = 0;
                        var height = 0;
                        if (image.Width >= 400 && image.Height >= 600)
                        {
                            width = image.Width / scaleRate.Value;
                            height = image.Height / scaleRate.Value;
                        }

                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Compand = compand,
                            Mode = resizeMode,
                            Position = anchorPosition,
                            Size = new SixLabors.Primitives.Size(width, height)
                        }));
                    }

                    image.Save(Path.Combine(directory, fileName));
                }
            }
        }

        public static void UploadImage(string path, string directory, string fileName)
        {
            for (int i = 0; i < 4; i++)
            {
                var quaility = 70;
                if (i == 1) quaility = 60;
                else if (i == 2) quaility = 50;
                else if (i == 3) quaility = 40;
                using (var image = new MagickImage(path))
                {
                    image.Resize(image.Width / 2, image.Height / 2);
                    image.Strip();
                    image.Quality = quaility;
                    image.Write(Path.Combine(directory, $"{quaility}_{fileName}"));
                }
            }
        }
    }
}
