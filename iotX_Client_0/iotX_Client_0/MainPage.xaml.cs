using iotX_Backend_Test;
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
            initX();
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
    }
}
