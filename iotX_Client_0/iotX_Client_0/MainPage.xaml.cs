using iotX_Backend_Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace iotX_Client_0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            //initX();
           initSpeech();
        }

        public async void initX()
        {
            MainInstance.Init();
            await MainInstance.SignUp("lumia_1", "lumia_1@iotx.com", "95123456");
            MainInstance.startTranmission();
            statusTbl.Text = "Done!";
        }

        public async void GPIOStatusSet(object sender, RoutedEventArgs e)
        {
            var obj = sender as ToggleSwitch;
            MainInstance.setGPIOstatus(int.Parse(obj.Tag.ToString()), obj.IsOn);
        }

        public async void initSpeech()
        {
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            var url = new Uri("ms-appx:///SRGS-Enhanced.grxml").ToString();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(url));
            var grammarFileConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionGrammarFileConstraint(file);
            speechRecognizer.UIOptions.ExampleText = @"Ex. 'blue background', 'green text'";
            speechRecognizer.Constraints.Add(grammarFileConstraint);
            var status = await speechRecognizer.CompileConstraintsAsync();
            speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
            speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            await speechRecognizer.ContinuousRecognitionSession.StartAsync(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionMode.Default);

        }

        private void SpeechRecognizer_StateChanged(Windows.Media.SpeechRecognition.SpeechRecognizer sender, Windows.Media.SpeechRecognition.SpeechRecognizerStateChangedEventArgs args)
        {
            var x = args.State;
        }

        private async void ContinuousRecognitionSession_ResultGenerated(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession sender, Windows.Media.SpeechRecognition.SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            var sResult = args.Result;
            //if (sResult.SemanticInterpretation.Properties.ContainsKey("TURNON")&&sResult.SemanticInterpretation.Properties["TURNON"][0].ToString() != "...")
            //{
            //    string objectX = sResult.SemanticInterpretation.Properties["TURNON"][0].ToString();
            //    var messageDialog = new Windows.UI.Popups.MessageDialog(objectX + " is turned on.", "Text spoken");
            //    await messageDialog.ShowAsync();
            //    return;
            //}
            //if (sResult.SemanticInterpretation.Properties.ContainsKey("TURNOFF") && sResult.SemanticInterpretation.Properties["TURNOFF"][0].ToString() != "...")
            //{
            //    string objectX = sResult.SemanticInterpretation.Properties["TURNOFF"][0].ToString();
            //    var messageDialog = new Windows.UI.Popups.MessageDialog(objectX + " is turned off.", "Text spoken");
            //    await messageDialog.ShowAsync();
            //    return;
            //}
            //if (sResult.SemanticInterpretation.Properties.ContainsKey("TURNONALL") && sResult.SemanticInterpretation.Properties["TURNONALL"][0].ToString() != "...")
            //{
            //    string objectX = sResult.SemanticInterpretation.Properties["TURNONALL"][0].ToString();
            //    var messageDialog = new Windows.UI.Popups.MessageDialog(String.Format("All {0} are turned on.",objectX));
            //    await messageDialog.ShowAsync();
            //    return;
            //}
            //if (sResult.SemanticInterpretation.Properties.ContainsKey("TURNOFFALL") && sResult.SemanticInterpretation.Properties["TURNOFFALL"][0].ToString() != "...")
            //{
            //    string objectX = sResult.SemanticInterpretation.Properties["TURNOFFALL"][0].ToString();
            //    var messageDialog = new Windows.UI.Popups.MessageDialog(String.Format("All {0} are turned off.", objectX));
            //    await messageDialog.ShowAsync();
            //    return;
            //}
            var messageDialogX = new Windows.UI.Popups.MessageDialog(sResult.Text);
            await messageDialogX.ShowAsync();
        }

        private void ContinuousRecognitionSession_Completed(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession sender, Windows.Media.SpeechRecognition.SpeechContinuousRecognitionCompletedEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
