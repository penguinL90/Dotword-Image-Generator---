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

        private int imageheight = 100;

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

        private int imagewidth = 100;

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

        private Generator generator;

        private bool iStbt;

        public bool Istbt
        {
            get { return iStbt; }
            set 
            {
                iStbt = value;
                OnPropertyChanged(nameof(Istbt));
            }
        }

        private bool WaitingForPrinting = false;

        private Dotword? _dotword;

        public MainWindow()
        {
            NowPath = string.Empty;
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
            if (generator is null) return;
            IsBtnsEnabled = false;
            await generator.SetImageGrayAsync(ImageWidth, ImageHeight * 25 / 19, NowPath);
            ResolutionChangeBtn.Visibility = Visibility.Collapsed;
            IsBtnsEnabled = true;
        }

        private async void AddImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (generator is null) return;
            NowPath = OpenFile.Open_File();
            await generator.SetImageGrayAsync(ImageWidth, ImageHeight * 25 / 19, NowPath);
        }

        private async void SummonImageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (generator is null) return;
            IsBtnsEnabled = false;
            WaitingForPrinting = true;
            DateTime starttime = DateTime.Now;
            _dotword = await generator.BufferToDotWordGrayAsync();
            DateTime endcaltime = DateTime.Now;
            if (_dotword is null)
            {
                status.Content = "Can't summon dotwords";
                IsBtnsEnabled = true;
                return;
            }
            if (_dotword.Words.Length <= 10_000_000)
            {
                DotWordImageShowGrayTxtBx.Text = _dotword.Words;
                IsBtnsEnabled = true;
                status.Content =
                    $"Cal time: {(endcaltime - starttime).TotalMilliseconds:F3} ms\n";
            }
            else
            {
                IsBtnsEnabled = true;
                status.Content =
                    $"Image is too big to display\nCal time: {(endcaltime - starttime).TotalMilliseconds:F3} ms\n";
            }
        }

        private async void SaveImageBtn_Click(object sender, RoutedEventArgs e)
        {
            IsBtnsEnabled = false;
            if (_dotword is null)
            {
                IsBtnsEnabled = true;
                return;
            }
            ImageSaver.SaveGrayAsync2(_dotword, new(imagewidth, imageheight));
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

        private void tbt_Checked(object sender, RoutedEventArgs e)
        {
            if (generator is not null)
            {
                BufferInfo info = generator.BufferInfo;
                generator = new ImageGenerator2x2(info);
            }
            else generator = new ImageGenerator2x2(null);
            fbf.IsChecked = false;
        }

        private void fbf_Checked(object sender, RoutedEventArgs e)
        {
            if (generator is not null)
            {
                BufferInfo info = generator.BufferInfo;
                generator = new ImageGenerator4x4(info);
            }
            else generator = new ImageGenerator4x4(null);
            tbt.IsChecked = false;
        }
    }
}