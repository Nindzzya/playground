using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace iotX_Backend_Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static ObservableCollection<bool> GPIOStatus = new ObservableCollection<bool>() { false, false, false, false };
        public MainPage()
        {
            this.InitializeComponent();
            MainInstance.Init();
            MainInstance.MessageBody = new System.Collections.ObjectModel.ObservableCollection<string>();
            InitX();
        }

        private async void InitX()
        {            
            await MainInstance.SignIn("kesavaprasadarul@outlook.com", "95123456");
            MainInstance.startTranmission();
            statusBlock.Text = "Done!";
        }
        public void ToggleSwitch(int pin)
        {
            Type classType = this.GetType();
            FieldInfo info = classType.GetField("GPIO"+pin);
            var obj = info.GetValue(this);
            (obj as ToggleSwitch).IsOn = !(obj as ToggleSwitch).IsOn;
        }

    }
}
