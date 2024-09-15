using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Globalization;

namespace dot_picture_generator.Class
{
    internal class ImageGenerator4x4 : Generator
    {
        public BufferInfo BufferInfo { get; set; }

        public ImageGenerator4x4(BufferInfo? info)
        {
            if (info is not null)
            {
                BufferInfo = info;
            }
        }

        public Task SetImageGrayAsync(int width, int height, string path)
        {
            return Task.Run(() =>
            {
                if (path == string.Empty)
                {
                    return;
                }

                byte[] buffer = new byte[width * height];
                BitmapImage bitmapImage = new();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(path);
                bitmapImage.DecodePixelWidth = width;
                bitmapImage.DecodePixelHeight = height;
                bitmapImage.EndInit();

                FormatConvertedBitmap convertedBitmap = new(bitmapImage, PixelFormats.Gray8, null, 0);
                convertedBitmap.CopyPixels(buffer, width, 0);
                BufferInfo = new(buffer, width, height);
            });
        }

        public Task<Dotword?> BufferToDotWordGrayAsync()
        {
            return Task.Run(() =>
            {
                if (BufferInfo?.Buffer == null)
                {
                    return Task.FromResult<Dotword?>(null);
                }

                char[] OutCharArray = new char[BufferInfo.Height * (BufferInfo.Width * 2 + 1)];

                Parallel.For(0, BufferInfo.Height, index =>
                {
                    int offset = index * BufferInfo.Width;
                    char[] bufferchar = new char[BufferInfo.Width * 2 + 1];
                    for (int i = 0; i < BufferInfo.Width; i++)
                    {
                        char[] unit = UnitToDotWord(BufferInfo.Buffer[offset + i]);
                        bufferchar[i * 2] = unit[0];
                        bufferchar[i * 2 + 1] = unit[1];
                    }
                    bufferchar[^1] = '\n';
                    Array.Copy(bufferchar, 0, OutCharArray, index * (BufferInfo.Width * 2 + 1), BufferInfo.Width * 2 + 1);
                });
                return Task.FromResult<Dotword?>(new Dotword(string.Join("", OutCharArray), DotwordType.FourByFour, BufferInfo.Height, BufferInfo.Width));
            });
        }

        private char[] UnitToDotWord(byte unit)
        {
            switch (unit)
            {
                case >= 0 and < 55:
                    return ['\u28FF', '\u28FF'];
                case >= 55 and < 148:
                    return ['\u288F', '\u2875'];
                case >= 148 and < 203:
                    return ['\u2851', '\u288C'];
                case >= 203 and < 237:
                    return ['\u2830', '\u2806'];
                case >= 237 and <= 255:
                    return ['\u2840', '\u2840'];
            }      
        }
    }
}
