﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace dot_picture_generator.Class
{
    internal static class ImageSaver
    {
        public static Task SaveGrayAsync(string data, int width = 4032, int height = 3024)
        {
            return Task.Run(() =>
            {
                if (data == string.Empty)
                {
                    return;
                }
                string path = OpenPath();
                if (path == string.Empty)
                {
                    return;
                }
                Thread thread = new(() =>
                {
                    TextBlock textBlock = new()
                    {
                        TextAlignment = System.Windows.TextAlignment.Center,
                        FontFamily = new("Consolas"),
                        Margin = new(100),
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        Text = data
                    };

                    Viewbox viewbox = new()
                    {
                        VerticalAlignment = System.Windows.VerticalAlignment.Stretch,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                        Child = textBlock
                    };

                    Grid grid = new();
                    grid.Children.Add(viewbox);
                    grid.Width = width;
                    grid.Height = height;
                    grid.Background = new SolidColorBrush(Colors.Transparent);

                    grid.Measure(new System.Windows.Size(width, height));
                    grid.Arrange(new System.Windows.Rect(new System.Windows.Size(width, height)));
                    grid.UpdateLayout();

                    RenderTargetBitmap render = new(width, height, 96, 96, PixelFormats.Default);
                    using Stream stream = File.Create(path);
                    render.Render(grid);
                    PngBitmapEncoder encoder = new();
                    encoder.Frames.Add(BitmapFrame.Create(render));
                    encoder.Save(stream);
                    stream.Dispose();
                    stream.Close();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = false;
                thread.Start();
                thread.Join();
            });
        }


        /// <summary>
        /// Not implemented well, Please don't use this api.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static Task SaveGrayAsync(string data, int fontSize)
        {
            return Task.Run(() =>
            {
                if (data == string.Empty)
                {
                    return;
                }
                string path = OpenPath();
                if (path == string.Empty)
                {
                    return;
                }
                Thread thread = new(() =>
                {
                    string[] split = data.Split('\n');
                    int height = split.Length * fontSize;
                    int width = split[0].Length * fontSize;

                    TextBlock textBlock = new();
                    textBlock.TextAlignment = System.Windows.TextAlignment.Center;
                    textBlock.FontFamily = new("Consolas");
                    textBlock.Margin = new(100);
                    textBlock.FontSize = fontSize;
                    textBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    textBlock.Text = data;
                    textBlock.Background = new SolidColorBrush(Colors.White);

                    textBlock.UpdateLayout();

                    RenderTargetBitmap render = new(width, height, 96, 96, PixelFormats.Default);
                    using Stream stream = File.Create(path);
                    render.Render(textBlock);
                    PngBitmapEncoder encoder = new();
                    encoder.Frames.Add(BitmapFrame.Create(render));
                    encoder.Save(stream);
                    stream.Dispose();
                    stream.Close();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            });
        }

        /// <summary>
        /// Not implemented will, Please don't use this api.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Task SaveRGB24Async(List<string> data, int width = 4032, int height = 3024)
        {
            return Task.Run(() =>
            {
                if (data == null)
                {
                    return;
                }
                string path = OpenPath();
                if (path == string.Empty)
                {
                    return;
                }
                Thread thread = new(() =>
                {
                    TextBlock textBlockA = new();
                    textBlockA.TextAlignment = System.Windows.TextAlignment.Center;
                    textBlockA.FontFamily = new("Consolas");
                    textBlockA.Margin = new(100);
                    textBlockA.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    textBlockA.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    textBlockA.Foreground = new SolidColorBrush(Colors.Red);
                    textBlockA.Opacity = 0.5;
                    textBlockA.Text = data[0];

                    Viewbox viewboxA = new();
                    viewboxA.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    viewboxA.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    viewboxA.Child = textBlockA;

                    TextBlock textBlockB = new();
                    textBlockB.TextAlignment = System.Windows.TextAlignment.Center;
                    textBlockB.FontFamily = new("Consolas");
                    textBlockB.Margin = new(100);
                    textBlockB.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    textBlockB.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    textBlockB.Foreground = new SolidColorBrush(Colors.Green);
                    textBlockB.Opacity = 0.7;
                    textBlockB.Text = data[1];


                    Viewbox viewboxB = new();
                    viewboxB.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    viewboxB.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    viewboxB.Child = textBlockB;

                    TextBlock textBlockC = new();
                    textBlockC.TextAlignment = System.Windows.TextAlignment.Center;
                    textBlockC.FontFamily = new("Consolas");
                    textBlockC.Margin = new(100);
                    textBlockC.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    textBlockC.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                    textBlockC.Foreground = new SolidColorBrush(Colors.Blue);
                    textBlockC.Opacity = 0.4;
                    textBlockC.Text = data[2];


                    Viewbox viewboxC = new();
                    viewboxC.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                    viewboxC.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    viewboxC.Child = textBlockC;

                    Grid grid = new();
                    grid.Children.Add(viewboxA);
                    grid.Children.Add(viewboxB);
                    grid.Children.Add(viewboxC);
                    grid.Width = width;
                    grid.Height = height;
                    grid.Background = new SolidColorBrush(Colors.BlanchedAlmond);

                    grid.Measure(new System.Windows.Size(width, height));
                    grid.Arrange(new System.Windows.Rect(new System.Windows.Size(width, height)));
                    grid.UpdateLayout();

                    RenderTargetBitmap render = new(width, height, 96, 96, PixelFormats.Default);
                    using Stream stream = File.Create(path);
                    render.Render(grid);
                    PngBitmapEncoder encoder = new();
                    encoder.Frames.Add(BitmapFrame.Create(render));
                    encoder.Save(stream);
                    stream.Dispose();
                    stream.Close();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            });
        }

        private static string OpenPath()
        {
            string Path = OpenFile.Open_Folder();
            if (Path == string.Empty)
            {
                return string.Empty;
            }
            return Path + $"\\{DateTime.UtcNow.ToFileTimeUtc()}.png";
        }
    }
}
