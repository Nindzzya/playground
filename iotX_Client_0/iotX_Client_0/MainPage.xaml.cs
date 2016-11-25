using iotX_Backend_Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.ExchangeActiveSyncProvisioning;
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

        }

        private async void button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainInstance.Init();
           await MainInstance.SignUp("lumia_1", "lumia_1@iotx.com", "95123456");
            MainInstance.startTranmission();
        }

        private void button2_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MainInstance.sendMessage();
        }
    }
}
