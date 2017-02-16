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
            App.client.MqttMsgPublishReceived -= client_MqttMsgPublishReceived;
            var senderItem = ((ToggleButton)sender).Name;
            App.client.Publish(topicName, Encoding.UTF8.GetBytes(((ToggleButton)sender).IsChecked == true ? String.Format("{0} on", senderItem) : String.Format("{0} off", senderItem)));
            App.client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        }
    }
}
