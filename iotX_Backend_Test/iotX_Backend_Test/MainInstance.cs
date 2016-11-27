using System.Threading.Tasks;
using Quickblox.Sdk;
using Quickblox.Sdk.Modules.UsersModule.Requests;
using Quickblox.Sdk.Modules.ChatXmppModule;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using Windows.Devices.Gpio;
using System.Diagnostics;

namespace iotX_Backend_Test
{
    public class MainInstance
    {
        GpioController gpio;
        static GpioPin[] pin = new GpioPin[4];
        public static UserRequest user { get; set; }
        public static string dId { get; set; }
        public static int recId { get; set; }
        public static PrivateChatManager privateChatManagerX { get; set; }
        public static ObservableCollection<string> MessageBody;
        public static ObservableCollection<string> Online;
        public static QuickbloxClient quickbloxClient = new QuickbloxClient(Keys.appId, Keys.authKey, Keys.authSecret, Keys.apiEndpoint, Keys.chatEndpoint);
        public static void Init()
        {
            
            Quickblox.Sdk.Platform.QuickbloxPlatform.Init();
        }

        public static async Task SignIn(string Email, string Password)
        {
            dId = null;
            var sessionResponse = await quickbloxClient.AuthenticationClient.CreateSessionBaseAsync();
            if (sessionResponse.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var loginResponse = await quickbloxClient.AuthenticationClient.ByEmailAsync(Email, Password);
                await quickbloxClient.ChatXmppClient.Connect(loginResponse.Result.User.Id, Password);
            }
            InitGPIO();
        }

        public static async void startTranmission()
        {
            quickbloxClient.ChatXmppClient.MessageReceived += async (object sender, MessageEventArgs messageEventArgs) =>
           {
               var message = ((System.Xml.Linq.XElement)messageEventArgs.Message.ExtraParameters.NextNode).Value;               
               var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
               var dMessage = (JObject)JsonConvert.DeserializeObject(message);
               var friendlyName = dMessage["FriendlyName"].ToString();
               await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (dMessage["Type"].ToString() == "setGPIOstatus")
                    {
                        setGPIO(dMessage);                        
                    }
                    if (dMessage["Type"].ToString() == "Init")
                    {
                        newOnline(friendlyName);
                    }
                }
              );

           };
        }
        private static void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            if (gpio == null)
            {
                Debugger.Break();
            }
            pin[0] = gpio.OpenPin(5);
            pin[1] = gpio.OpenPin(6); //13,19
            pin[2] = gpio.OpenPin(13);
            pin[3] = gpio.OpenPin(19);
            pin[0].Write(GpioPinValue.High);
            pin[1].Write(GpioPinValue.High);
            pin[2].Write(GpioPinValue.High);
            pin[3].Write(GpioPinValue.High);
            pin[0].SetDriveMode(GpioPinDriveMode.Output);
            pin[1].SetDriveMode(GpioPinDriveMode.Output);
            pin[2].SetDriveMode(GpioPinDriveMode.Output);
            pin[3].SetDriveMode(GpioPinDriveMode.Output);

        }
        private async static void newOnline (string friendlyName)
        {
            Online.Add(friendlyName + " is Online");
        }
        private async static void setGPIO(JObject dMessage)
        {
            string pinX = dMessage["GPIOPin"].ToString();
            string bitX = dMessage["bit"].ToString();
            MessageBody.Add(pinX + "-" + bitX);

            if (bitX=="True")
            {
                pin[int.Parse(pinX)-1].Write(GpioPinValue.Low);
            }
            else if (bitX=="False")
            {
                pin[int.Parse(pinX) - 1].Write(GpioPinValue.High);
            }
        }
        public async static void sendStatus(string message)
        {

        }

    }


}
