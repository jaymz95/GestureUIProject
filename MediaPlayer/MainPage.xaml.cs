using System;
using System.Collections.Generic;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private FaceDetectionEffect _faceDetectionEffect;
        private MediaCapture _mediaCapture;
        private IMediaEncodingProperties _previewProperties;
        //public var file;#
        private IStorageFile file;
        private bool pauseButton = false;
        private bool pauseCommand = false;
        private bool playCommand = false;

        private async void btnCamera_Click(object sender, RoutedEventArgs e)
        {
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();
            cePreview.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
            detectFaces_Click(sender, e);
        }

        private async void detectFaces_Click(object sender, RoutedEventArgs e)
        {
            var faceDetectionDefinition = new FaceDetectionEffectDefinition();
            faceDetectionDefinition.DetectionMode = FaceDetectionMode.HighPerformance;
            faceDetectionDefinition.SynchronousDetectionEnabled = false;
            _faceDetectionEffect = (FaceDetectionEffect)await
            _mediaCapture.AddVideoEffectAsync(faceDetectionDefinition,
              MediaStreamType.VideoPreview);
            _faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;
            _faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
            _faceDetectionEffect.Enabled = true;
        }

        private async void FaceDetectionEffect_FaceDetected(
            FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            var detectedFaces = args.ResultFrame.DetectedFaces;

            await Dispatcher
              .RunAsync(CoreDispatcherPriority.Normal,
                () => DrawFaceBoxes(detectedFaces));
        }

        private void DrawFaceBoxes(IReadOnlyList<DetectedFace> detectedFaces)
        {
            cvsFaceOverlay.Children.Clear();


            //MediaPlaybackSession playbackSession = sender as MediaPlaybackSession;
            Debug.WriteLine(detectedFaces.Count);
            if (file != null)
            {
                if (detectedFaces.Count <= 0 || playCommand == false)// && mediaPlayer.MediaPlayer.CurrentStateChanged += MediaElementState.Paused)
                {
                    if(playCommand == true && detectedFaces.Count <= 0)
                    {
                        mediaPlayer.MediaPlayer.Play();
                    }
                    else
                    {
                        mediaPlayer.MediaPlayer.Pause();
                    }
                }
                else if (detectedFaces.Count > 0 && pauseCommand == false)
                {
                    mediaPlayer.MediaPlayer.Play();
                }
            }

            for (int i = 0; i < detectedFaces.Count; i++)
            {
                var face = detectedFaces[i];

                var faceBounds = face.FaceBox;
                Rectangle faceHighlightRectangle = new Rectangle()
                {
                    Height = faceBounds.Height,
                    Width = faceBounds.Width
                };
                Canvas.SetLeft(faceHighlightRectangle, faceBounds.X);
                Canvas.SetTop(faceHighlightRectangle, faceBounds.Y);
                faceHighlightRectangle.StrokeThickness = 2;
                faceHighlightRectangle.Stroke = new SolidColorBrush(Colors.Red);
                cvsFaceOverlay.Children.Add(faceHighlightRectangle);
            }
        }

        private Rectangle MapRectangleToDetectedFace(BitmapBounds detectedfaceBoxCoordinates)
        {
            var faceRectangle = new Rectangle();
            var previewStreamPropterties =
              _previewProperties as VideoEncodingProperties;
            double mediaStreamWidth = previewStreamPropterties.Width;
            double mediaStreamHeight = previewStreamPropterties.Height;
            var faceHighlightRect = LocatePreviewStreamCoordinates(previewStreamPropterties,
              this.cePreview);
            faceRectangle.Width = (detectedfaceBoxCoordinates.Width / mediaStreamWidth) *
              faceHighlightRect.Width;
            faceRectangle.Height = (detectedfaceBoxCoordinates.Height / mediaStreamHeight) *
              faceHighlightRect.Height;
            var x = (detectedfaceBoxCoordinates.X / mediaStreamWidth) *
              faceHighlightRect.Width;
            var y = (detectedfaceBoxCoordinates.Y / mediaStreamHeight) *
              faceHighlightRect.Height;
            Canvas.SetLeft(faceRectangle, x);
            Canvas.SetTop(faceRectangle, y);
            return faceRectangle;
        }
        public Rect LocatePreviewStreamCoordinates(
          VideoEncodingProperties previewResolution,
          CaptureElement previewControl)
        {
            var uiRectangle = new Rect();
            var mediaStreamWidth = previewResolution.Width;
            var mediaStreamHeight = previewResolution.Height;
            uiRectangle.Width = previewControl.ActualWidth;
            uiRectangle.Height = previewControl.ActualHeight;
            var uiRatio = previewControl.ActualWidth / previewControl.ActualHeight;
            var mediaStreamRatio = mediaStreamWidth / mediaStreamHeight;
            if (uiRatio > mediaStreamRatio)
            {
                var scaleFactor = previewControl.ActualHeight / mediaStreamHeight;
                var scaledWidth = mediaStreamWidth * scaleFactor;
                uiRectangle.X = (previewControl.ActualWidth - scaledWidth) / 2.0;
                uiRectangle.Width = scaledWidth;
            }
            else
            {
                var scaleFactor = previewControl.ActualWidth / mediaStreamWidth;
                var scaledHeight = mediaStreamHeight * scaleFactor;
                uiRectangle.Y = (previewControl.ActualHeight - scaledHeight) / 2.0;
                uiRectangle.Height = scaledHeight;
            }
            return uiRectangle;
        }


        private SpeechRecognizer speechRecognizer;

        private string Player;
        private int xPos;
        private int yPos;

        public MainPage()
        {
            this.InitializeComponent();
            Player = "";
            xPos = 0;
            yPos = 0;
            this.Loaded += MainPage_Loaded;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await SetLocalMedia();
        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");
            openPicker.FileTypeFilter.Add(".mkv");
            openPicker.FileTypeFilter.Add(".avi");

            file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaPlayerElement defined in XAML
            if (file != null)
            {
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);

                mediaPlayer.MediaPlayer.Play();

            }
        }

        // Play the media.


        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            speechRecognizer = new SpeechRecognizer();
            speechRecognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(0);
            speechRecognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(5);
            speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(0.5);

            // load the grammar file
            var grammarFile = await StorageFile.GetFileFromApplicationUriAsync(
                    new Uri("ms-appx:///grammar.xml"));

            // add the grammar to the constraints for the speech engine
            speechRecognizer.Constraints.Add(
                new SpeechRecognitionGrammarFileConstraint(grammarFile));

            // need to run this anyway, even without adding a grammar
            var result = await speechRecognizer.CompileConstraintsAsync();
            if (result.Status == SpeechRecognitionResultStatus.Success)
            {
                while (true)
                {
                    SpeechRecognitionResult srr = await speechRecognizer.RecognizeAsync();

                    // try and parse to get some meaningful input
                    // following crashes without an if to check
                    //string myCommand = 
                    //        srr.SemanticInterpretation.Properties["command"].Single();

                    // safer way to get the information 
                    // Action - delegate method
                    // set up a dictionary of string/Action pairs
                    string myCommand = "No command found";
                    if ((srr.Confidence == SpeechRecognitionConfidence.High) ||
                        (srr.Confidence == SpeechRecognitionConfidence.Medium) ||
                        (srr.Confidence == SpeechRecognitionConfidence.Low))
                    {
                        IReadOnlyList<string> myCommands;
                        if (srr.SemanticInterpretation.Properties.TryGetValue(
                                    "command",
                                    out myCommands) == true)
                        {
                            myCommand = "Command: " + myCommands.FirstOrDefault();
                            if (myCommands.FirstOrDefault() == "play")
                            {
                                mediaPlayer.MediaPlayer.Play();
                                playCommand = true;
                                pauseCommand = false;
                            }
                            else if (myCommands.FirstOrDefault() == "pause")
                            {
                                mediaPlayer.MediaPlayer.Pause();
                                pauseCommand = true;
                                playCommand = false;
                            }
                        }
                        if (srr.SemanticInterpretation.Properties.TryGetValue(
                                    "player",
                                    out myCommands) == true)
                        {
                            Player = myCommands.FirstOrDefault();
                        }
                        if (srr.SemanticInterpretation.Properties.TryGetValue(
                                    "row",
                                    out myCommands) == true)
                        {
                            xPos = Convert.ToInt32(myCommands.FirstOrDefault());
                        }
                        if (srr.SemanticInterpretation.Properties.TryGetValue(
                                    "col",
                                    out myCommands) == true)
                        {
                            yPos = Convert.ToInt32(myCommands.FirstOrDefault());
                        }

                        if (Player != "")
                        {
                            OnMakeMove();
                        }

                    } // end if srr.confidencce


                    var messageDialog = new Windows.UI.Popups.MessageDialog(
                            myCommand, "Spoken Text");
                    await messageDialog.ShowAsync();

                }
            }
            else
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(
                        "Grammar not loaded", "Spoken Text");
                await messageDialog.ShowAsync();
            }
        }

        private async void OnMakeMove()
        {
            var messageDialog = new Windows.UI.Popups.MessageDialog(
                    "Moving " + Player + " to Row: " + xPos + ", Column: " + yPos,
                    "Spoken Text");
            await messageDialog.ShowAsync();
        }
    }
}


