using Microsoft.Win32;
using openalprnet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PlatesRecognition.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<ResultViewModel> ResultsList
        {
            get;
            set;
        }
        public MediaElement SourceMediaElement
        {
            get
            {
                return _sourceMediaElement;
            }
            set
            {
                if (_sourceMediaElement != value)
                {
                    OnPropertyChanged(nameof(SourceMediaElement));
                    _sourceMediaElement = value;
                }

            }
        }

        public double VideoDuration
        {
            get
            {
                return _videoDuration;

            }
            set
            {
                if (_videoDuration != value)
                {
                    OnPropertyChanged(nameof(VideoDuration));
                    _videoDuration = value;
                }
            }
        }
        public double VideoAcutalPosition
        {
            get
            {
                return _videoAcutalPosition;

            }
            set
            {
                if (_videoAcutalPosition != value)
                {
                    OnPropertyChanged(nameof(VideoAcutalPosition));
                    _videoAcutalPosition = value;
                }
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName != value)
                {
                    OnPropertyChanged(nameof(FileName));
                    _fileName = value;
                }
            }
        }

        public string LastRecognizedPlate
        {
            get
            {
                return _lastRecognizedPlate;

            }
            set
            {

                OnPropertyChanged(nameof(LastRecognizedPlate));
                _lastRecognizedPlate = value;

            }
        }
        private readonly DispatcherTimer _videoTimer;

        private readonly DispatcherTimer _processTimer;

        private readonly DispatcherTimer _lineTimer;

        private readonly Config _config;

        private readonly AlprNet _process;
        private MediaElement _sourceMediaElement;
        private double _videoDuration;
        private double _videoAcutalPosition;
        private string _lastRecognizedPlate;
        private string _fileName;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            var tempHandler = PropertyChanged;
            if (tempHandler != null)
            {
                tempHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public MainView()
        {
            _videoTimer = new DispatcherTimer();
            _videoTimer.Interval = TimeSpan.FromMilliseconds(200);
            _videoTimer.Tick += VideoTimer_Tick;

            _processTimer = new DispatcherTimer();
            _processTimer.Interval = TimeSpan.FromMilliseconds(20); //0,2 sec
            _processTimer.Tick += _processTimer_Tick;


            _lineTimer = new DispatcherTimer();
            _lineTimer.Interval = TimeSpan.FromMilliseconds(500); //0,5 sec
            _lineTimer.Tick += _lineTimer_Tick;


            ResultsList = new ObservableCollection<ResultViewModel>();

            _config = new Config();

            _process = new AlprNet(_config.Country, _config.ConfigFile, _config.RuntimeDir);


            SourceMediaElement = new MediaElement();

            SourceMediaElement.LoadedBehavior = MediaState.Manual;
            SourceMediaElement.UnloadedBehavior = MediaState.Manual;




            InitializeComponent();



        }

        private void _lineTimer_Tick(object sender, EventArgs e)
        {
            HideLines();
            _lineTimer.Stop();
        }

        private void _processTimer_Tick(object sender, EventArgs e)
        {
            if (SourceMediaElement.Position.Seconds < 1) return;



            var w = SourceMediaElement.ActualWidth;
            var h = SourceMediaElement.ActualHeight;

            System.Drawing.Size dpi = new System.Drawing.Size(96, 96);

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
                if (plate.BestPlate.Characters.Length >= 7)
                {
                    if (plate.BestPlate.OverallConfidence > 85)
                    {
                        ResultsList.Add(new ResultViewModel(plate.BestPlate.Characters, SourceMediaElement.Position, plate.BestPlate.OverallConfidence, true));
                        LastRecognizedPlate = plate.BestPlate.Characters;
                    }
                    else
                    {
                        ResultsList.Add(new ResultViewModel(plate.BestPlate.Characters, SourceMediaElement.Position, plate.BestPlate.OverallConfidence));
                    }


                    _lineTimer.Start();

                }

                SetFirstLines(plate);
                SetSecondLines(plate);
                SetThirdLines(plate);
                SetFourthLines(plate);

            }


        }

        private void SetFirstLines(AlprPlateResultNet plate)
        {
            FirstLine.X1 = plate.PlatePoints[0].X;
            FirstLine.Y1 = plate.PlatePoints[0].Y;

            FirstLine.X2 = plate.PlatePoints[1].X;
            FirstLine.Y2 = plate.PlatePoints[1].Y;
        }
        private void SetSecondLines(AlprPlateResultNet plate)
        {
            SecondLine.X1 = plate.PlatePoints[1].X;
            SecondLine.Y1 = plate.PlatePoints[1].Y;


            SecondLine.X2 = plate.PlatePoints[2].X;
            SecondLine.Y2 = plate.PlatePoints[2].Y;
        }
        private void SetThirdLines(AlprPlateResultNet plate)
        {
            ThirdLine.X1 = plate.PlatePoints[2].X;
            ThirdLine.Y1 = plate.PlatePoints[2].Y;

            ThirdLine.X2 = plate.PlatePoints[3].X;
            ThirdLine.Y2 = plate.PlatePoints[3].Y;
        }
        private void SetFourthLines(AlprPlateResultNet plate)
        {
            FourthLine.X1 = plate.PlatePoints[3].X;
            FourthLine.Y1 = plate.PlatePoints[3].Y;


            FourthLine.X2 = plate.PlatePoints[0].X;
            FourthLine.Y2 = plate.PlatePoints[0].Y;
        }

        private void HideLines()
        {
            FirstLine.X1 = FirstLine.X2 = FirstLine.Y2 = FirstLine.Y2 = 5000;
            SecondLine.X1 = SecondLine.X2 = SecondLine.Y2 = SecondLine.Y2 = 5000;
            ThirdLine.X1 = ThirdLine.X2 = ThirdLine.Y2 = ThirdLine.Y2 = 5000;
            FourthLine.X1 = FourthLine.X2 = FourthLine.Y2 = FourthLine.Y2 = 5000;
        }

        private void VideoTimer_Tick(object sender, EventArgs e)
        {
            if (SourceMediaElement.Source != null)
            {

                if (SourceMediaElement.NaturalDuration.HasTimeSpan)
                {
                    VideoDuration = SourceMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    VideoAcutalPosition = (SourceMediaElement.Position.TotalSeconds / VideoDuration) * 100;
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

                    //SplashScreenImage.Source = new BitmapImage(new Uri(dialog.FileName, UriKind.RelativeOrAbsolute));

                }
                else
                {
                    FileName = System.IO.Path.GetFileName(dialog.FileName);
                    SourceMediaElement.Source = new Uri(dialog.FileName, UriKind.RelativeOrAbsolute);
                    //  SplashScreenImage.Visibility = Visibility.Collapsed;
                    VideoTimeTextBlock.Visibility = Visibility.Visible;
                    ResultsList.Clear();
                    SourceMediaElement.Play();
                    if (SourceMediaElement.NaturalDuration.HasTimeSpan)
                    {
                        VideoDuration = SourceMediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                    }

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

    }
}
