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

namespace iotX_Backend_Test
{
    public class MainInstance
    {
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

        }

        public static async void startTranmission()
        {
            quickbloxClient.ChatXmppClient.MessageReceived +=  (object sender, MessageEventArgs messageEventArgs) =>
            {
                var message = ((System.Xml.Linq.XElement)messageEventArgs.Message.ExtraParameters.NextNode).Value;
                var friendlyName = messageEventArgs.Message.SenderId;
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                var dMessage =(JObject) JsonConvert.DeserializeObject(message);
                 dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
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
        private async static void newOnline (int friendlyName)
        {
            Online.Add(friendlyName.ToString() + " is Online");
        }
        private async static void setGPIO(JObject dMessage)
        {
            string pinX = dMessage["GPIOPin"].ToString();
            string bitX = dMessage["bit"].ToString();
            MessageBody.Add(pinX + "-" + bitX);
        }
        public async static void sendStatus(string message)
        {

        }

    }


}
