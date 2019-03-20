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
        //public static void UploadImage(byte[] byteArray, string directory, string fileName,
        //    bool compand = true, ResizeMode resizeMode = ResizeMode.Max, AnchorPositionMode anchorPosition = AnchorPositionMode.Center)
        //{
        //    using (Stream uploadStream = new MemoryStream(byteArray))
        //    {
        //        using (var image = Image.Load(uploadStream))
        //        {
        //            if (!Directory.Exists(directory))
        //            {
        //                Directory.CreateDirectory(directory);
        //            }

        //            var dimenisions = GetDimensions(image.Width, image.Height, byteArray);

        //            if (dimenisions.Width > 0 && dimenisions.Height > 0)
        //            {
        //                image.Mutate(x => x.Resize(new ResizeOptions
        //                {
        //                    Compand = compand,
        //                    Mode = resizeMode,
        //                    Position = anchorPosition,
        //                    Size = new SixLabors.Primitives.Size(dimenisions.Width, dimenisions.Height)
        //                }));
        //            }

        //            image.Save(Path.Combine(directory, fileName));
        //        }
        //    }
        //}

        public static void UploadImage(byte[] byteArray, string directory, string fileName)
        {
            using (MagickImage image = new MagickImage(byteArray))
            {
                var percentage = GetPercentage(byteArray);
                image.Resize(new Percentage(percentage));
                image.Write(Path.Combine(directory, fileName));
            }
        }

        private static (int Width, int Height) GetDimensions(int originalWidth, int originalHeight, byte[] byteArray)
        {
            var width = 0;
            var height = 0;
            double sizeInMb = (double)byteArray.Length / (1024 * 1024);
            if (originalWidth < 500)
            {
                width = originalWidth;
                height = originalHeight;
            }
            else if (500 <= originalWidth && originalWidth <= 600)
            {
                width = (int)(originalWidth / 1.1);
                height = (int)(originalHeight / 1.1);
            }
            else if (600 < originalWidth && originalWidth <= 700)
            {
                width = (int)(originalWidth / 1.2);
                height = (int)(originalHeight / 1.2);
            }
            else if (700 < originalWidth && originalWidth <= 800)
            {
                if (sizeInMb > 0.5 && sizeInMb <= 0.7)
                {
                    width = (int)(originalWidth / 1.2);
                    height = (int)(originalHeight / 1.2);
                }
                else
                {
                    width = (int)(originalWidth / 1.3);
                    height = (int)(originalHeight / 1.3);
                }
            }
            else if (800 < originalWidth && originalWidth <= 900)
            {
                if (sizeInMb <= 0.5)
                {
                    width = (int)(originalWidth / 1.4);
                    height = (int)(originalHeight / 1.4);
                }
                else
                {
                    width = (int)(originalWidth / 1.5);
                    height = (int)(originalHeight / 1.5);
                }
            }
            else if (900 < originalWidth && originalWidth <= 1000)
            {
                if (sizeInMb <= 0.5)
                {
                    width = (int)(originalWidth / 1.5);
                    height = (int)(originalHeight / 1.5);
                }
                else if (0.5 < sizeInMb && sizeInMb <= 0.7)
                {
                    width = (int)(originalWidth / 1.7);
                    height = (int)(originalHeight / 1.7);
                }
                else
                {
                    width = originalWidth / 2;
                    height = originalHeight / 2;
                }
            }
            else
            {
                if (sizeInMb <= 1)
                {
                    width = (int)(originalWidth / 1.5);
                    height = (int)(originalHeight / 1.5);
                }
                else if (sizeInMb > 1 && sizeInMb <= 2)
                {
                    width = originalWidth / 2;
                    height = originalHeight / 2;
                }
                else if (1000 < originalWidth && originalWidth <= 1500)
                {
                    width = originalWidth / 3;
                    height = originalHeight / 3;
                }
                else if (1500 < originalWidth && originalWidth <= 2000)
                {
                    width = originalWidth / 4;
                    height = originalHeight / 4;
                }
                else if (2000 < originalWidth && originalWidth <= 3000)
                {
                    width = originalWidth / 5;
                    height = originalHeight / 5;
                }
                else if (3000 < originalWidth)
                {
                    if (2 < sizeInMb && sizeInMb < 3)
                    {
                        width = originalWidth / 5;
                        height = originalHeight / 5;
                    }
                    else
                    {
                        width = originalWidth / 7;
                        height = originalHeight / 7;
                    }
                }
            }

            return (width, height);
        }

        private static double GetPercentage(byte[] byteArray)
        {
            double percentage = 100;
            double sizeInMb = (double)byteArray.Length / (1024 * 1024);

            if (sizeInMb <= 0.4) percentage = 100;
            else if (sizeInMb > 0.4 && sizeInMb <= 0.5) percentage = 95;
            else if (sizeInMb > 0.5 && sizeInMb <= 0.7) percentage = 90;
            else if (sizeInMb > 0.7 && sizeInMb < 1) percentage = 85;
            else if (sizeInMb >= 1 && sizeInMb <= 1.5) percentage = 70;
            else if (sizeInMb > 1.5 && sizeInMb <= 2) percentage = 65;
            else if (sizeInMb > 2 && sizeInMb <= 3) percentage = 60;
            else if (sizeInMb > 3 && sizeInMb <= 4) percentage = 55;
            else if (sizeInMb > 4 && sizeInMb <= 5) percentage = 50;
            else if (sizeInMb > 5) percentage = 40;

            return percentage;
        }
    }
}
