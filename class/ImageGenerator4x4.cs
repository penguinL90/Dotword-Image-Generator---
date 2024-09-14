using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static dot_picture_generator.Class.ImageGenerator2x2;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Globalization;

namespace dot_picture_generator.Class
{
    internal class ImageGenerator4x4
    {
        byte[] Buffer;
        string Path;
        int BufferWidth;
        int BufferHeight;

        public Task SetImageGrayAsync(int width, int height, string path)
        {
            return Task.Run(() =>
            {
                if (path == string.Empty)
                {
                    return;
                }

                BufferWidth = width;
                BufferHeight = height;
                Path = path;

                Buffer = new byte[width * height];
                BitmapImage bitmapImage = new();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(path);
                bitmapImage.DecodePixelWidth = width;
                bitmapImage.DecodePixelHeight = height;
                bitmapImage.EndInit();

                FormatConvertedBitmap convertedBitmap = new(bitmapImage, PixelFormats.Gray8, null, 0);
                convertedBitmap.CopyPixels(Buffer, width, 0);
            });
        }

        public Task<string> BufferToDotWordGrayAsync()
        {
            return Task.Run(() =>
            {
                if (Buffer == null)
                {
                    return Task.FromResult("Can't Summon Dot Word Image.");
                }

                char[] OutCharArray = new char[BufferHeight * (BufferWidth * 2 + 1)];

                Parallel.For(0, BufferHeight, index =>
                {
                    int offset = index * BufferWidth;
                    char[] bufferchar = new char[BufferWidth * 2 + 1];
                    for (int i = 0; i < BufferWidth; i++)
                    {
                        char[] unit = UnitToDotWord(Buffer[offset + i]);
                        bufferchar[i * 2] = unit[0];
                        bufferchar[i * 2 + 1] = unit[1];
                    }
                    bufferchar[^1] = '\n';
                    Array.Copy(bufferchar, 0, OutCharArray, index * (BufferWidth * 2 + 1), BufferWidth * 2 + 1);
                });
                return Task.FromResult(string.Join("", OutCharArray));
            });
        }

        private char[] UnitToDotWord(byte unit)
        {
            const byte divide = 255 / 5;
            switch (unit)
            {
                case >= 0 and < divide:
                    return ['\u28FF', '\u28FF'];
                case >= divide and < divide * 2:
                    return ['\u288F', '\u2875'];
                case >= divide * 2 and < divide * 3:
                    return ['\u2851', '\u288C'];
                case >= divide * 3 and < divide * 4:
                    return ['\u2830', '\u2806'];
                case >= divide * 4 and <= divide * 5:
                    return ['\u2840', '\u2840'];
            }      
        }
    }
}
