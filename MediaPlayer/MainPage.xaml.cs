using System;
using System.Collections.Generic;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.FaceAnalysis;
using Windows.Media.MediaProperties;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    
    public sealed partial class MainPage : Page
    {

        private FaceDetectionEffect _faceDetectionEffect;
        private MediaCapture _mediaCapture;
        private IMediaEncodingProperties _previewProperties;
        //public var file;#
        private IStorageFile file;
        private IStorageFile ile;
        private bool pauseButton = false;
        private bool pauseCommand = false;
        private bool playCommand = false;
        private bool voicePause = false;
        private static int stateChange = 0;

        // Create a timer and set a two second interval.
        private static Timer aTimer = new System.Timers.Timer();
        private static Timer bTimer = new System.Timers.Timer();
        private static int counter = 0;
        private static SpeechSynthesizer synthesizer;
        private static SpeechSynthesisStream synthesisStream;
        private static bool errorFaceDetect = false;
        private static bool faceDetect = true;


        private async void faceDetectOff_Click(object sender, RoutedEventArgs e)
        {
            
            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
            var messageDialog = new Windows.UI.Popups.MessageDialog(
                            "Would you like to turn off face detection controls to Play and Pause?");
            messageDialog.Commands.Add(new UICommand("Yes", (command) =>
            {
                faceDetect = false;
            }));

            messageDialog.Commands.Add(new UICommand("No", (command) =>
            {
                faceDetect = true;
            }));
            await messageDialog.ShowAsync();
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

        public static void timedCommands()
        {
            aTimer.Interval = 1000;
            // Hook up the Elapsed event for the timer. 
            //aTimer.Elapsed += OnTimedEvent;

            aTimer.Elapsed += (sender, e) => OnTimedEvent(sender, e);

            if (counter >= 30)
            {
                // or whatever your limit is
                if (stateChange >= 5)
                {
                    errorFaceDetect = true;
                }
                stateChange = 0;
                counter = 0;
            }

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            counter++;
            if (counter >= 30)
            {
                // or whatever your limit is
                if (stateChange >= 5)
                {
                    errorFaceDetect = true;
                    Debug.WriteLine("ERRRRRRRRRRRROOOORRRRRRRRRRRRRRRRRRR");
                    //var mp = (MainPage)frame.Content;
                    //mp.
                    if (errorFaceDetect == true)
                    {
                        
                    }

                }
                stateChange = 0;
                counter = 0;
            }
            Debug.WriteLine("Counter: " + counter);
        }


        private void DrawFaceBoxes(IReadOnlyList<DetectedFace> detectedFaces)
        {
            cvsFaceOverlay.Children.Clear();

            //MediaPlaybackSession playbackSession = sender as MediaPlaybackSession;
            //Debug.WriteLine(detectedFaces.Count);
            if (file != null && faceDetect == true)
            {
                if (detectedFaces.Count <= 0 && playCommand == true)// && mediaPlayer.MediaPlayer.CurrentStateChanged += MediaElementState.Paused)
                {
                    mediaPlayer.MediaPlayer.Pause();
                    pauseCommand = true;
                    playCommand = false;
                    stateChange++;
                    Debug.WriteLine("stateChange: " + stateChange);

                }
                else if (detectedFaces.Count > 0 && playCommand == false && voicePause == false)
                {
                    mediaPlayer.MediaPlayer.Play();
                    playCommand = true;
                    pauseCommand = false;
                    stateChange++;
                    Debug.WriteLine("stateChange: " + stateChange);
                }
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
        private static MainPage mp;

        public MainPage()
        {
            this.InitializeComponent();
            Player = "";
            xPos = 0;
            yPos = 0;
            this.Loaded += MainPage_Loaded;
            mp = this;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();
            cePreview.Source = _mediaCapture;
            await _mediaCapture.StartPreviewAsync();
            detectFaces_Click(sender, e);
            timedCommands();
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

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

            synthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            synthesisStream = await synthesizer.SynthesizeTextToStreamAsync("Would you like to turn off face detection controls to Play and Pause?");

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
                                stateChange++;
                                Debug.WriteLine("stateChange: " + stateChange);
                                playCommand = true;
                                pauseCommand = false;
                                voicePause = false;
                            }
                            else if (myCommands.FirstOrDefault() == "pause")
                            {
                                mediaPlayer.MediaPlayer.Pause();
                                stateChange++;
                                Debug.WriteLine("stateChange: " + stateChange);
                                pauseCommand = true;
                                playCommand = false;
                                voicePause = true;
                            }
                            if (myCommands.FirstOrDefault() == "fastForward")
                            {
                                mediaPlayer.MediaPlayer.PlaybackSession.Position = mediaPlayer.MediaPlayer.PlaybackSession.Position.Add(
                                  new TimeSpan(0, 0, 30));
                            }
                            if (myCommands.FirstOrDefault() == "rewind")
                            {
                                mediaPlayer.MediaPlayer.PlaybackSession.Position = mediaPlayer.MediaPlayer.PlaybackSession.Position.Add(
                                  new TimeSpan(0, 0, -30));
                            }
                        }

                    }


                    var messageDialog = new Windows.UI.Popups.MessageDialog(
                            myCommand, "Spoken Text");
                    //await messageDialog.ShowAsync();

                }
            }
            else
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog(
                        "Grammar not loaded", "Spoken Text");
                await messageDialog.ShowAsync();
            }
        }

    }
}


