using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using openalprnet;
using Size = System.Drawing.Size;

namespace PlatesRecognition
{
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _videoTimer;

        private readonly DispatcherTimer _processTimer;

        private readonly Config _config;

        private readonly AlprNet _process;

        private readonly ObservableCollection<ResultViewModel> _resultsList;

        public MainWindow()
        {
            InitializeComponent();

            _videoTimer = new DispatcherTimer();
            _videoTimer.Interval = TimeSpan.FromSeconds(1);
            _videoTimer.Tick += VideoTimer_Tick;

            _processTimer = new DispatcherTimer();
            _processTimer.Interval = TimeSpan.FromMilliseconds(500); //0,5 sec
            _processTimer.Tick += _processTimer_Tick;

            SourceMediaElement.LoadedBehavior = MediaState.Manual;
            SourceMediaElement.UnloadedBehavior = MediaState.Manual;

            _resultsList = new ObservableCollection<ResultViewModel>();

            _config = new Config();

            _process = new AlprNet(_config.Country, _config.ConfigFile, _config.RuntimeDir);
        }

        private void _processTimer_Tick(object sender, EventArgs e)
        {
            if(SourceMediaElement.Position.Seconds < 1) return;
            
            Size dpi = new Size(96, 96);
            var w = SourceMediaElement.ActualWidth;
            var h = SourceMediaElement.ActualHeight;
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)w, (int)h, dpi.Width, dpi.Height, PixelFormats.Pbgra32);

            bmp.Render(SourceMediaElement);


            MemoryStream stream = new MemoryStream();
            BitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));
            encoder.Save(stream);

            Bitmap bitmap = new Bitmap(stream);

            // try to recognize image
            AlprResultsNet result = _process.Recognize(bitmap);

            foreach (var plate in result.Plates)
            {
                _resultsList.Insert(0, new ResultViewModel()
                {
                    PlateCharacters = plate.BestPlate.Characters,
                    TimeSpan = SourceMediaElement.Position
                });
            }

            ResultsListView.ItemsSource = _resultsList;
        }

        private void VideoTimer_Tick(object sender, EventArgs e)
        {
            if (SourceMediaElement.Source != null)
            {
                if (SourceMediaElement.NaturalDuration.HasTimeSpan)
                {
                    // check if finish
                    if (SourceMediaElement.Position == SourceMediaElement.NaturalDuration.TimeSpan)
                    {
                        SourceMediaElement.Stop();
                        _videoTimer.Stop();
                        _processTimer.Stop();
                    }
                    else
                    {
                        var position = SourceMediaElement.Position.ToString(@"mm\:ss");
                        var wholeTime = SourceMediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                        VideoTimeTextBlock.Text = $"{position}/{wholeTime}";
                    }
                }
            }
        }

        #region Menu

        private void OpenFile_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "mp4 files (*.mp4)|*.mp4|avi files (*.avi)|*.avi|all files (*.*)|*.*";

            var dialogResult = dialog.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                // if photo file
                if (dialog.FileName.EndsWith(".jpg"))
                {
                    Bitmap bitmap = new Bitmap(dialog.FileName);

                    SplashScreenImage.Source = new BitmapImage(new Uri(dialog.FileName, UriKind.RelativeOrAbsolute));

                }
                else
                {

                    SourceMediaElement.Source = new Uri(dialog.FileName, UriKind.RelativeOrAbsolute);
                    SplashScreenImage.Visibility = Visibility.Collapsed;
                    VideoTimeTextBlock.Visibility = Visibility.Visible;
                    SourceMediaElement.Play();
                    _videoTimer.Start();
                    _processTimer.Start();
                }
            }

        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {

            //todo: clear all handlers from video

            _videoTimer.Stop();

            _processTimer.Stop();

            Application.Current.Shutdown();

        }

        private void Export_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void Help_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Pomoc");
        }

        #endregion
    }
}
