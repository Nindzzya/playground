using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt.Messages;
using Windows.ApplicationModel.Core;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Networking.Connectivity;
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
        public static bool isConnected = false;
        public static string topicName = "/iotX/values";
        public MainPage()
        {
            this.InitializeComponent();
            NetworkInformation_NetworkStatusChanged(null);
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
            if (isConnected)
            {
                App.client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
                byte[] qosSpecifier = { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE };
                App.client.Subscribe(new string[] { topicName }, qosSpecifier);
            }
            //initSpeech();

            GpioController gpio = GpioController.GetDefault();
            if (gpio == null)
                return; // GPIO not available on this system

            // Open GPIO 5
            using (GpioPin pin = gpio.OpenPin(5))
            {
                // Latch HIGH value first. This ensures a default value when the pin is set as output
                pin.Write(GpioPinValue.High);

                // Set the IO direction as output
                pin.SetDriveMode(GpioPinDriveMode.Output);

            } // Close pin - will revert to its power-on stat
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            var connection = NetworkInformation.GetConnectionProfiles();
            foreach (var con in connection)
            {
               if(con != null && con.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess);
                {
                    isConnected = true;
                    return;
                }            
            }
            isConnected = false;
            
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var response = Encoding.UTF8.GetString(e.Message);
            string[] split = response.Split(' ');
            respondtoResponse(split);            
        }
        private async void respondtoResponse(string[] resp)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            if (resp[0].Contains("switch"))
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ToggleButton switchX = (ToggleButton)FindName(resp[0]);
                    switchX.IsChecked= resp[1] == "on" ? true : false;
                });              

            }
            else if (resp[0].Contains("fan"))
            {

               await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ToggleButton fanX = (ToggleButton)FindName(resp[0]);
                    fanX.IsChecked = resp[1] == "on" ? true : false;                   
                });
            }
        }
        private void switch1_Click(object sender, RoutedEventArgs e)
        {
            var senderItem = ((ToggleButton)sender).Name;
            var send = Encoding.UTF8.GetBytes(((ToggleButton)sender).IsChecked == true ? String.Format("{0} on", senderItem) : String.Format("{0} off", senderItem));
            publishPost(send);
        }
        private void publishPost(byte[] send)
        {
            if (isConnected)
                App.client.Publish(topicName, send);

        }
        public async void initSpeech()
        {
            var speechRecognizer = new SpeechRecognizer();
            var url = new Uri("ms-appx:///SRGS-Enhanced V2.grxml").ToString();
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(url));
            var grammarFileConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionGrammarFileConstraint(file);
            speechRecognizer.Timeouts.EndSilenceTimeout =new TimeSpan(0,0,0,0,400);
            speechRecognizer.Constraints.Add(grammarFileConstraint);
            var status = await speechRecognizer.CompileConstraintsAsync();
            speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
            speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            await speechRecognizer.ContinuousRecognitionSession.StartAsync(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionMode.Default);

        }

        private async void ContinuousRecognitionSession_ResultGenerated(Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession sender, Windows.Media.SpeechRecognition.SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            var sResult = args.Result;
            await processInfo(sResult);
        }

        public async Task processInfo(SpeechRecognitionResult result)
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
                else { await new Windows.UI.Popups.MessageDialog("Core Components in speech are missing, please retry.").ShowAsync(); }

            }
        }

        public void switchObj(string obj, bool state, bool isAll, string location = null, int number = 0)
        {
            switch (obj)
            {
                case "switch":
                    if (isAll)
                    {
                        publishPost(Encoding.UTF8.GetBytes("switch1 on"));
                        publishPost(Encoding.UTF8.GetBytes("switch2 on"));
                        publishPost(Encoding.UTF8.GetBytes("switch3 on"));
                        publishPost(Encoding.UTF8.GetBytes("switch4 on"));
                        return;
                    }
                    switch (number)
                    {
                        case 1:
                            publishPost(Encoding.UTF8.GetBytes("switch1 on"));
                            return;
                        case 2:
                            publishPost(Encoding.UTF8.GetBytes("switch2 on")); return;
                        case 3:
                            publishPost(Encoding.UTF8.GetBytes("switch3 on")); return;
                        case 4:
                            publishPost(Encoding.UTF8.GetBytes("switch4 on")); return;
                        default:
                            break;
                    }

                    break;
                case "fan":
                    if (isAll)
                    {
                        publishPost(Encoding.UTF8.GetBytes("fan1 on"));
                        publishPost(Encoding.UTF8.GetBytes("fan2 on"));
                        return;
                    }
                    switch (number)
                    {
                        case 1:
                            publishPost(Encoding.UTF8.GetBytes("fan1 on"));
                            return;
                        case 2:
                            publishPost(Encoding.UTF8.GetBytes("fan2 on"));
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
