using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace dot_picture_generator.Class
{
    internal class ImageGenerator2x2
    {

		private string path = string.Empty;
		public string Path
		{
			get { return path; }
			set 
			{ 
				path = value;
			}
		}

        private ColorPriority priority;
        public ColorPriority ColorPriority
        {
            get { return priority; }
            set { priority = value; }
        }


        private byte[] Buffer;
		private int BufferWidth;
		private int BufferHeight;
        private bool IsFullColor;

		public Status NowStatus { get; set; }

		public ImageGenerator2x2()
		{
			NowStatus = Status.Ready;
            ColorPriority = new(6, 2, 4, 7);
		}

		public Task SetImageGrayAsync(int width, int height, string path)
		{
			return Task.Run(() =>
			{
                NowStatus = Status.ConvertingToBuffer;
                if (path == string.Empty)
                {
                    NowStatus = Status.Failed;
					return;
                }

                IsFullColor = false;
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

                NowStatus = Status.CompletedToBuffer;
            });
		}

        public Task SetImageRGB24Async(int width, int height, string path)
        {
            return Task.Run(() =>
            {
                NowStatus = Status.ConvertingToBuffer;
                if (path == string.Empty)
                {
                    NowStatus = Status.Failed;
                    return;
                }

                IsFullColor = true;
                BufferWidth = width;
                BufferHeight = height;
                Path = path;

                Buffer = new byte[width * height * 3];
                BitmapImage bitmapImage = new();

                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(path);
                bitmapImage.DecodePixelWidth = width;
                bitmapImage.DecodePixelHeight = height;
                bitmapImage.EndInit();

                FormatConvertedBitmap convertedBitmap = new(bitmapImage, PixelFormats.Rgb24, null, 0);
                convertedBitmap.CopyPixels(Buffer, width * 3, 0);

                NowStatus = Status.CompletedToBuffer;
            });
        }

        public Task<string> BufferToDotWordGrayAsync()
		{
			return Task.Run(() =>
			{
                if (Buffer == null || IsFullColor)
                {
                    NowStatus = Status.Failed;
                    return Task.FromResult("Can't Summon Dot Word Image.");
                }
                while (NowStatus == Status.ConvertingToBuffer) ;
                NowStatus = Status.ConvertingToDotWord;

                List<byte[][]> ChunkedList = Enumerable.Chunk(Enumerable.Chunk(Buffer, BufferWidth), 2).ToList();

                char[] OutCharArray = new char[ChunkedList.Count * (BufferWidth + 1)];

                Parallel.For(0, ChunkedList.Count, index =>
                {
                    byte[][] bufferbyte = ChunkedList[index];
                    char[] bufferchar = new char[BufferWidth + 1];
                    int height = bufferbyte.Length;
                    byte[] unit = new byte[height];
                    for (int i = 0; i < BufferWidth; i++)
                    {
                        Array.Fill<byte>(unit, 255);
                        for (int j = 0; j < height; j++)
                        {
                            unit[j] = bufferbyte[j][i];
                        }
                        bufferchar[i] = UnitToDotWord(unit, height);
                    }
                    bufferchar[^1] = '\n';
                    Array.Copy(bufferchar, 0, OutCharArray, index * (BufferWidth + 1), BufferWidth + 1);
                });
                NowStatus = Status.Completed;
                return Task.FromResult(string.Join("", OutCharArray));
            });
		}

        public Task<List<string>> BufferToDotWordRGB24Async()
        {
            return Task.Run(() =>
            {
                if (Buffer == null || !IsFullColor)
                {
                    NowStatus = Status.Failed;
                    return Task.FromResult<List<string>>(["Can't Summon Dot Word Image.", "Can't Summon Dot Word Image.", "Can't Summon Dot Word Image."]);
                }
                while (NowStatus == Status.ConvertingToBuffer) ;
                NowStatus = Status.ConvertingToDotWord;

                List<byte[][]> ChunkedList = Enumerable.Chunk(Enumerable.Chunk(Buffer, BufferWidth * 3), 2).ToList();

                char[] OutCharArray_R = new char[ChunkedList.Count * (BufferWidth + 1)];
                char[] OutCharArray_G = new char[ChunkedList.Count * (BufferWidth + 1)];
                char[] OutCharArray_B = new char[ChunkedList.Count * (BufferWidth + 1)];

                Parallel.For(0, ChunkedList.Count, index =>
                {
                    byte[][] bufferbyte = ChunkedList[index];
                    char[][] bufferchar =
                    [
                        new char[BufferWidth + 1],
                        new char[BufferWidth + 1],
                        new char[BufferWidth + 1],
                    ];
                    int height = bufferbyte.Length;
                    byte[] unit = new byte[height];
                    for (int i = 0; i < BufferWidth * 3; i += 3)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            Array.Fill<byte>(unit, 255);
                            for (int j = 0; j < height; j++)
                            {
                                unit[j] = bufferbyte[j][i + x];
                            }
                            bufferchar[x][i / 3] = UnitToDotWord(unit, height);
                        }
                    }
                    bufferchar[0][^1] = '\n';
                    bufferchar[1][^1] = '\n';
                    bufferchar[2][^1] = '\n';
                    Array.Copy(bufferchar[0], 0, OutCharArray_R, index * (BufferWidth + 1), BufferWidth + 1);
                    Array.Copy(bufferchar[1], 0, OutCharArray_G, index * (BufferWidth + 1), BufferWidth + 1);
                    Array.Copy(bufferchar[2], 0, OutCharArray_B, index * (BufferWidth + 1), BufferWidth + 1);
                });
                NowStatus = Status.Completed;
                return Task.FromResult<List<string>>([string.Join("", OutCharArray_R), string.Join("", OutCharArray_G), string.Join("", OutCharArray_B)]);
            });
        }

        private char UnitToDotWord(byte[] unit, int height)
		{            
			int up = 0;
			int down = 0;
			int result;

			for (int i = 0; i < height; i++)
			{
                int value = unit[i];
                if (value >= 0 && value < priority.Black)
                {
                    if (i == 0)
                    {
                        up += 0b1011;
                        down += 0b0001;
                    }
                    else if (i == 1)
                    {
                        down += 0b1110;
                        up += 0b0100;
                    }
                }
                else if (value >= priority.Black && value < priority.Mid + priority.Black)
                {
                    if (i == 0)
                    {
                        up += 0b0011;
                        down += 0b0001;
                    }
                    else if (i == 1)
                    {
                        up += 0b0100;
                        down += 0b0110;
                    }
                }
                else if (value >= priority.Mid + priority.Black 
                    && value < priority.Mid + priority.Mid2 + priority.Black)
                {                   
                    if (i == 0) up += 0b1010;
                    else if (i == 1) down += 0b0110;
                }				
			}
            if (up + down == 0)
            {
                down = 8;
            }
            result = 2 * 16 * 16 * 16 + 8 * 16 * 16 + down * 16 + up;
            return Convert.ToChar(result);
        }

		public enum Status
		{
			Ready,
			ConvertingToBuffer,
			CompletedToBuffer,
			ConvertingToDotWord,
			Completed,
			Failed,
		}

	}
    internal struct ColorPriority
        {
            private double black;
            private double white;
            private double mid;
            private double mid2;
            public double Black { get =>  black;}
            public double White { get => white;}
            public double Mid { get => mid;}
            public double Mid2 { get => mid2; }
            public ColorPriority(int _black, int _mid, int _mid2, int _white)
            {
                double total = _black + _mid + _white + _mid2;
                black = _black / total * 255;
                white = _white / total * 255;
                mid = _mid / total * 255;
                mid2 = _mid2 / total * 255;
            }
        }
}
