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

                    var dimenisions = GetDimensions(image.Width, image.Height, byteArray);

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
                width = (int)(originalWidth * 0.4);
                height = (int)(originalHeight * 0.4);
            }
            else if (1 <= sizeInMb && sizeInMb <= 3)
            {
                width = originalWidth / 3;
                height = originalHeight / 3;
            }
            else if (3 < sizeInMb && sizeInMb <= 5)
            {
                width = originalWidth / 4;
                height = originalHeight / 4;
            }
            else if (5 < sizeInMb)
            {
                width = originalWidth / 5;
                height = originalHeight / 5;
            }

            return (width, height);
        }

        private static (int Width, int Height) GetDimensions(int originalWidth, int originalHeight, byte[] byteArray = null)
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
                width = (int)(originalWidth / 1.2);
                height = (int)(originalHeight / 1.2);
            }
            else if (600 < originalWidth && originalWidth <= 700)
            {
                width = (int)(originalWidth / 1.3);
                height = (int)(originalHeight / 1.3);
            }
            else if (700 < originalWidth && originalWidth <= 800)
            {
                width = (int)(originalWidth / 1.5);
                height = (int)(originalHeight / 1.5);
            }
            else if (800 < originalWidth && originalWidth <= 900)
            {
                if (sizeInMb <= 0.5)
                {
                    width = (int)(originalWidth / 1.5);
                    height = (int)(originalHeight / 1.5);
                }
                else
                {
                    width = originalWidth / 2;
                    height = originalHeight / 2;
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
                    width = originalWidth / 2;
                    height = originalHeight / 2;
                }
                else
                {
                    width = originalWidth / 3;
                    height = originalHeight / 3;
                }
            }
            else
            {
                if (sizeInMb <= 1)
                {
                    width = originalWidth / 2;
                    height = originalHeight / 2;
                }
                else if (sizeInMb > 1 && sizeInMb <= 2)
                {
                    width = originalWidth / 3;
                    height = originalHeight / 3;
                }
                else if (1000 < originalWidth && originalWidth <= 1500)
                {
                    width = originalWidth / 4;
                    height = originalHeight / 4;
                }
                else if (1500 < originalWidth && originalWidth <= 2000)
                {
                    width = originalWidth / 5;
                    height = originalHeight / 5;
                }
                else if (2000 < originalWidth && originalWidth <= 3000)
                {
                    width = originalWidth / 7;
                    height = originalHeight / 7;
                }
                else if (3000 < originalWidth)
                {
                    if (2 < sizeInMb && sizeInMb < 3)
                    {
                        width = originalWidth / 6;
                        height = originalHeight / 6;
                    }
                    else
                    {
                        width = originalWidth / 8;
                        height = originalHeight / 8;

                    }
                }
            }

            return (width, height);
        }
    }
}
