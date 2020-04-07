using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MediaPlayer
{
    class Recognition
    {
        public void jjAsync(SpeechSynthesisStream synthesisStream, Frame frame)
        {
            //var synthesizer = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

            // Create a stream from the text. This will be played using a media element.
            //SpeechSynthesisStream synthesisStream = await synthesizer.SynthesizeTextToStreamAsync("stop stop stop");

            //MediaElement media = ;
            // Set the source and start playing the synthesized audio stream.

            var page = (MainPage)frame.Content;

            //MainPage m = new MainPage();
            MediaElement media = page.MyMedia;


            media.AutoPlay = true;
            media.SetSource(synthesisStream, synthesisStream.ContentType);
            media.Play();
        }
    }
}
