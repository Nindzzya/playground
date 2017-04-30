using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using static UMS_Alpha.Core.Academic.StudentModule;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UMS_Alpha
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<Student> students = new List<Student>();
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog dialog = new ContentDialog();
            TextBox ntxtBox = new TextBox();
            TextBox rtxtBox = new TextBox();
            Button AcceptBtn = new Button();
            AcceptBtn.Content = "Accept";
            AcceptBtn.Click += (s,ex)=> {
                students.Add(new Student(ntxtBox.Text, rtxtBox.Text));
                dialog.Hide();
            };
            StackPanel stkPanel = new StackPanel();
            stkPanel.Children.Add(ntxtBox);
            stkPanel.Children.Add(rtxtBox);
            stkPanel.Children.Add(AcceptBtn);
            dialog.Content = stkPanel;
            var result = await dialog.ShowAsync();
        }
    }
}
