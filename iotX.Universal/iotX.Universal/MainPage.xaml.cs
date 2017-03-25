using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace iotX.Universal
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static string topicName = "/iotX/values";
        public MainPage()
        {
            this.InitializeComponent();
            App.client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            byte[] qosSpecifier = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
            App.client.Subscribe(new string[] {topicName}, qosSpecifier );
            //initSpeech();
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var response = Encoding.UTF8.GetString(e.Message);
            string[] split = response.Split(' ');
            respondtoResponse(split);            
        }
        private void respondtoResponse(string[] resp)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (resp[0].Contains("switch"))
            {
                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    switch (resp[0].Replace("switch", ""))
                    {
                        case "1":
                            switch1.IsChecked = resp[1] == "on" ? true : false;
                            break;
                        case "2":
                            switch2.IsChecked = resp[1] == "on" ? true : false;
                            break;
                        case "3":
                            switch3.IsChecked = resp[1] == "on" ? true : false;
                            break;
                        case "4":
                            switch4.IsChecked = resp[1] == "on" ? true : false;
                            break;
                    }
                });
            }
            else if (resp[0].Contains("fan"))
            {
                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    switch (resp[0].Replace("fan", ""))
                    {
                        case "1":
                            fan1.IsChecked = resp[1] == "on" ? true : false;
                            break;
                        case "2":
                            fan2.IsChecked = resp[1] == "on" ? true : false;
                            break;
                    }
                });
            }
        }
        private void switch1_Click(object sender, RoutedEventArgs e)
        {
            var senderItem = ((ToggleButton)sender).Name;
            var send = Encoding.UTF8.GetBytes(((ToggleButton)sender).IsChecked == true ? String.Format("{0} on", senderItem) : String.Format("{0} off", senderItem));
            publishPost(send);
        }
        private async void publishPost(byte[] send)
        {
            App.client.Publish(topicName, send);

        }
        public async void initSpeech()
        {

            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();
            var x = speechRecognizer.CurrentLanguage;
            var url = new Uri(String.Format("ms-appx:///SRGS-Enhanced-{0}.grxml",x.LanguageTag)).ToString();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(url));
            var grammarFileConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionGrammarFileConstraint(file);
            speechRecognizer.Timeouts.EndSilenceTimeout =new TimeSpan(0,0,0,0,400);
            speechRecognizer.Constraints.Add(grammarFileConstraint);            
            var status = await speechRecognizer.CompileConstraintsAsync();
            speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
            speechRecognizer.StateChanged += SpeechRecognizer_StateChanged;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            await speechRecognizer.ContinuousRecognitionSession.StartAsync(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionMode.Default);

        }

        private void SpeechRecognizer_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            if (args.State == SpeechRecognizerState.SoundEnded)
                throw new Exception();
            var x = 1;
        }

        private async void ContinuousRecognitionSession_ResultGenerated(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession sender, Windows.Media.SpeechRecognition.SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            var sResult = args.Result;
            processInfo(sResult);
        }

        public void processInfo(SpeechRecognitionResult result)
        {
            var props = result.SemanticInterpretation.Properties;
            if (checkifKey(props, "state"))
            {
                var setState = props["state"].First();
                if (checkifKey(props, "object"))
                {
                    var setObject = props["object"].First();
                    string setLocation = null;
                    int number = 0;
                    if (checkifKey(props, "number"))
                        number = returnNumber(props["number"].First());
                    if (checkifKey(props, "location"))
                        setLocation = props["location"].First();

                    switch (setState)
                    {

                        case "on":
                            switchObj(setObject, true, false, setLocation,number);
                            break;
                        case "off":
                            switchObj(setObject, false, false, setLocation, number);
                            break;
                        case "on all":
                            switchObj(setObject, true, true, setLocation, number);
                            break;
                        case "off all":
                            switchObj(setObject, false, true, setLocation, number);
                            break;
                    }
                }
                else { new Windows.UI.Popups.MessageDialog("Core Components in speech are missing, please retry.").ShowAsync(); }

            }
        }

        public async void switchObj(string obj, bool state, bool isAll, string location = null, int number = 0)
        {
            switch (obj)
            {
                case "switch":                    
                        if(isAll)
                        {
                        if (state == true)
                        {
                            publishPost(Encoding.UTF8.GetBytes("switch1 on"));
                            publishPost(Encoding.UTF8.GetBytes("switch2 on"));
                            publishPost(Encoding.UTF8.GetBytes("switch3 on"));
                            publishPost(Encoding.UTF8.GetBytes("switch4 on"));
                        }
                        else
                        {
                            publishPost(Encoding.UTF8.GetBytes("switch1 off"));
                            publishPost(Encoding.UTF8.GetBytes("switch2 off"));
                            publishPost(Encoding.UTF8.GetBytes("switch3 off"));
                            publishPost(Encoding.UTF8.GetBytes("switch4 off"));
                        }
                        return;
                        }
                        switch (number)
                        {
                            case 1:
                            if (state == true)
                                publishPost(Encoding.UTF8.GetBytes("switch1 on"));
                            else
                                    publishPost(Encoding.UTF8.GetBytes("switch1 off"));
                                return;
                            case 2:
                            if (state == true)
                                publishPost(Encoding.UTF8.GetBytes("switch2 on"));
                            else
                                publishPost(Encoding.UTF8.GetBytes("switch2 off"));
                            return;
                        case 3:
                            if (state == true)
                                publishPost(Encoding.UTF8.GetBytes("switch3 on"));
                            else
                                publishPost(Encoding.UTF8.GetBytes("switch3 off"));
                            return;
                        case 4:
                            if (state == true)
                                publishPost(Encoding.UTF8.GetBytes("switch4 on"));
                            else
                                publishPost(Encoding.UTF8.GetBytes("switch4 off"));
                            return;
                        default:
                                break;
                        }

                    break;
                case "fan":
                    if (isAll)
                    {
                        if (state == true)
                        {
                            publishPost(Encoding.UTF8.GetBytes("fan1 on"));
                            publishPost(Encoding.UTF8.GetBytes("fan2 on"));
                        }
                        else
                        {
                            publishPost(Encoding.UTF8.GetBytes("fan1 off"));
                            publishPost(Encoding.UTF8.GetBytes("fan2 off"));
                        }
                        return;
                    }
                    switch (number)
                    {
                        case 1:
                            if (state == true)
                            {
                                publishPost(Encoding.UTF8.GetBytes("fan1 on"));
                            }
                            else
                            {
                                publishPost(Encoding.UTF8.GetBytes("fan1 off"));
                            }
                            return;
                        case 2:
                            if (state == true)
                            {
                                publishPost(Encoding.UTF8.GetBytes("fan2 on"));
                            }
                            else
                            {
                                publishPost(Encoding.UTF8.GetBytes("fan2 off"));
                            }
                            return;
                        default:
                            break;
                    }

                    break;
                default:
                    break;
            }
        }

        public bool checkifKey(IReadOnlyDictionary<string, IReadOnlyList<string>> x, string check)
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
        private int returnNumber(string input)
        {
            switch (input)
            {
                case "one":
                    return 1;
                case "two":
                    return 2;
                case "three":
                    return 3;
                case "four":
                    return 4;
                case "five":
                    return 5;
                case "six":
                    return 6;
                case "seven":
                    return 7;
                case "eight":
                    return 8;
                case "nine":
                    return 9;
                default:
                    return 0;

            }

        }
    }
}
