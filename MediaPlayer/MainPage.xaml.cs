using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaPlayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
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

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaPlayerElement defined in XAML
            if (file != null)
            {
                mediaPlayer.Source = MediaSource.CreateFromStorageFile(file);

                mediaPlayer.MediaPlayer.Play();
            }
        }


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
                        (srr.Confidence == SpeechRecognitionConfidence.Medium))
                    {
                        IReadOnlyList<string> myCommands;
                        if (srr.SemanticInterpretation.Properties.TryGetValue(
                                    "command",
                                    out myCommands) == true)
                        {
                            myCommand = "Command: " + myCommands.FirstOrDefault();
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


