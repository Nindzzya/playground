using iotX_Backend_Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using System;
using Windows.UI.Xaml.Controls.Primitives;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace iotX_Client_0
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SpriteVisual effectVisual;

        public MainPage()
        {
            this.InitializeComponent();
            //initX();
           initSpeech();
        }

        public async void initX()
        {
            MainInstance.Init();
            await MainInstance.SignUp("lumia_2core", "lumia_2core@iotx.com", "95123456");
            MainInstance.startTranmission();
            LoadGrid.Visibility = Visibility.Collapsed;
            //statusTbl.Text = "Done!";
        }

        public async void GPIOStatusSet(object sender, RoutedEventArgs e)
        {
            var obj = sender as ToggleButton;
            bool checkedState = obj.IsChecked.GetValueOrDefault() == true ? true : false;
            MainInstance.setGPIOstatus(int.Parse(obj.Tag.ToString()), checkedState);
        }
         public async void initSpeech()
        {
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            var url = new Uri("ms-appx:///SRGS-Enhanced V2.grxml").ToString();
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
            processInfo(sResult); 
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

        }

        public void processInfo(SpeechRecognitionResult result)
        {
            var props = result.SemanticInterpretation.Properties;
            if(checkifKey(props,"state"))
            {
                var setState = props["state"].First();
                if(checkifKey(props,"object"))
                {
                    var setObject = props["object"].First();
                    string setLocation = null;
                    if (checkifKey(props, "location"))
                        setLocation = props["location"].First();

                    switch (setState)
                    {

                        case "on":
                            switchObj(setObject, true, false, setLocation);
                            break;
                        case "off":
                            switchObj(setObject, false, false, setLocation);
                            break;
                        case "on all":
                            switchObj(setObject, true, true, setLocation);
                            break;
                        case "off all":
                            switchObj(setObject, false, true, setLocation);
                            break;
                    }
                }
                else { new Windows.UI.Popups.MessageDialog("Core Components in speech are missing, please retry.").ShowAsync(); }

            }
        }

        public async void switchObj(string obj, bool state, bool isAll, string location = null)
        {
            var messageDialogX = new Windows.UI.Popups.MessageDialog(string.Format("{0} {1} {2} {3} turned {4}."
                , isAll == true ? "All" : "The"
                , isAll == true ? obj + "s" : obj
                , location == null ? "" : string.Format("at the {0}", location),
                isAll == true ? "are" : "is",
                state == true ? "on" : "off"));
            await messageDialogX.ShowAsync();
        }

        public bool checkifKey(IReadOnlyDictionary<string,IReadOnlyList<string>> x,string check)
        {
            if (x.ContainsKey(check) && x[check][0].ToString() != "...")
            {
                return true;
            }
            else
                return false;
        }

        private void ContinuousRecognitionSession_Completed(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession sender, Windows.Media.SpeechRecognition.SpeechContinuousRecognitionCompletedEventArgs args)
        {
            throw new NotImplementedException();
        }

    }
}
