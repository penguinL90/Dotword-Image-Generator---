using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using dot_picture_generator.Class;
using System.Text.RegularExpressions;

namespace dot_picture_generator
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        [GeneratedRegex("[^0-9]+")]
        public static partial Regex regex();

        private int imageheight = 1137;

        public int ImageHeight
        {
            get { return imageheight; }
            set
            {
                if (imageheight != value)
                {
                    if (NowPath != string.Empty) 
                        ResolutionChangeBtn.Visibility = Visibility.Visible;
                    if (value < 4) imageheight = 4;
                    else if (value > 10000) imageheight = 10000;
                    else imageheight = value;
                    OnPropertyChanged(nameof(ImageHeight));
                }
            }
        }

        private int imagewidth = 639;

        public int ImageWidth
        {
            get { return imagewidth; }
            set
            {
                if (imagewidth != value)
                {
                    if (NowPath != string.Empty) 
                        ResolutionChangeBtn.Visibility = Visibility.Visible;
                    if (value < 4) imagewidth = 4;
                    else if (value > 10000) imagewidth = 10000;
                    else imagewidth = value;
                    OnPropertyChanged(nameof(ImageWidth));
                }
            }
        }

        private string dotwords = string.Empty;

        public string DotWords
        {
            get { return dotwords; }
            set 
            {
                if (dotwords != value)
                {
                    dotwords = value;
                    OnPropertyChanged(nameof(DotWords));
                }
                else WaitingForPrinting = false;
            }
        }

        private string NowPath;

        private bool isbtnsenabled = true;

        public bool IsBtnsEnabled
        {
            get { return isbtnsenabled; }
            set 
            {
                isbtnsenabled = value;
                OnPropertyChanged(nameof(IsBtnsEnabled));
            }
        }

        private ImageGenerator4x4 generator;

        private bool WaitingForPrinting = false;

        public MainWindow()
        {
            NowPath = string.Empty;
            generator = new();
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ResolutionChangeBtn_Click(object sender, RoutedEventArgs e)
        {
            IsBtnsEnabled = false;
            await generator.SetImageGrayAsync(ImageWidth, ImageHeight * 25 / 19, NowPath);
            ResolutionChangeBtn.Visibility = Visibility.Collapsed;
            IsBtnsEnabled = true;
        }

        private async void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            NowPath = OpenFile.Open_File();
            await generator.SetImageGrayAsync(ImageWidth, ImageHeight * 25 / 19, NowPath);
        }

        private async void SummonImageBtn_Click(object sender, RoutedEventArgs e)
        {
            IsBtnsEnabled = false;
            WaitingForPrinting = true;
            DateTime starttime = DateTime.Now;
            string buffer = await generator.BufferToDotWordGrayAsync();
            DateTime endcaltime = DateTime.Now;
            if (buffer.Length > 10_000_000)
            {
                DotWords = "Image is too big to display.\nIt'll be downloaded later.";
                await Task.Delay(2000);
                await ImageSaver.SaveGrayAsync(buffer);
            }
            else
            {
                DotWords = buffer;
            }
            IsBtnsEnabled = true;
            status.Content =
                $"Cal time: {(endcaltime - starttime).TotalMilliseconds:F3} ms\n";
        }

        private async void SaveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            IsBtnsEnabled = false;
            await ImageSaver.SaveGrayAsync(DotWords);
            IsBtnsEnabled = true;
        }

        private void TxtBx_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex().IsMatch(e.Text);
        }

        private void TxtBx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DotWordImageShowGrayTxtBx.Focus();
            }
        }
    }
}