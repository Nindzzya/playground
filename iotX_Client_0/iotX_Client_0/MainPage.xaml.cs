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
            initX();
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
            var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(@"/SRGS-list.grxml",UriKind.Relative));
            var grammarFileConstraint = new Windows.Media.SpeechRecognition.SpeechRecognitionGrammarFileConstraint(storageFile, "colors");

        }
    }
}
