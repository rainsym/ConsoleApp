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
            bool compand = true, ResizeMode resizeMode = ResizeMode.Max, AnchorPositionMode anchorPosition = AnchorPositionMode.Center)
        {
            using (Stream uploadStream = new MemoryStream(byteArray))
            {
                using (var image = Image.Load(uploadStream))
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var dimenisions = GetDimensions(byteArray, image.Width, image.Height);

                    if (dimenisions.Width > 0 && dimenisions.Height > 0)
                    {
                        image.Mutate(x => x.Resize(new ResizeOptions
                        {
                            Compand = compand,
                            Mode = resizeMode,
                            Position = anchorPosition,
                            Size = new SixLabors.Primitives.Size(dimenisions.Width, dimenisions.Height)
                        }));
                    }

                    image.Save(Path.Combine(directory, fileName));
                }
            }
        }

        public static void UploadImage(string path, string directory, string fileName)
        {
            var byteArray = File.ReadAllBytes(path);
            for (int i = 0; i < 4; i++)
            {
                var quaility = 10;
                if (i == 1) quaility = 15;
                else if (i == 2) quaility = 20;
                else if (i == 3) quaility = 25;
                using (var image = new MagickImage(path))
                {
                    var dimenisions = GetDimensions(byteArray, image.Width, image.Height);
                    image.Resize(dimenisions.Width, dimenisions.Height);
                    image.Strip();
                    image.Quality = quaility;
                    image.Write(Path.Combine(directory, $"{quaility}_{fileName}"));
                }
            }
        }

        private static (int Width, int Height) GetDimensions(byte[] byteArray, int originalWidth, int originalHeight)
        {
            double sizeInMb = (double)byteArray.Length / (1024 * 1024);
            var width = 0;
            var height = 0;
            if (0.2 < sizeInMb && sizeInMb < 1)
            {
                width = (int)(originalWidth * 0.3);
                height = (int)(originalHeight * 0.3);
            }
            else if (1 <= sizeInMb && sizeInMb <= 3)
            {
                width = originalWidth / 2;
                height = originalHeight / 2;
            }
            else if (3 < sizeInMb && sizeInMb <= 5)
            {
                width = originalWidth / 3;
                height = originalHeight / 3;
            }
            else if (5 < sizeInMb)
            {
                width = originalWidth / 4;
                height = originalHeight / 4;
            }

            return (width, height);
        }
    }
}
