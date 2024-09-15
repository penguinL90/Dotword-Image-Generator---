using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dot_picture_generator.Class
{
    internal interface Generator
    {
        public Task<Dotword?> BufferToDotWordGrayAsync();
        public Task SetImageGrayAsync(int width, int height, string path);
        public BufferInfo BufferInfo { get; set; }
    }
    internal class BufferInfo(byte[]? buffer, int w, int h)
    {
        public byte[]? Buffer = buffer;
        public int Width = w;
        public int Height = h;
    }
}
