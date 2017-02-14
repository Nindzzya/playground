using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MQTTTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Timer ipTimer;
        public MainPage()
        {
            this.InitializeComponent();
            statusTb.Text = "Initializing..";
            ipTimer = new Timer(updateIp, null, (int)TimeSpan.FromHours(1).TotalMilliseconds, Timeout.Infinite);
            updateIp(null);
        }

        public async void updateIp(object state)
        {
            await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
      () => { statusTb.Text = "Updating Local IP..."; });
            var getip = new Windows.Web.Http.HttpClient();
            var htmlbody = await getip.GetStringAsync(new Uri("http://checkip.dyndns.org/"));
            var substring = htmlbody.ToString().Replace("<html><head><title>Current IP Check</title></head><body>Current IP Address: ", "");
            var internetip = substring.ToString().Replace("</body></html>\r\n", "");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue(
        "Basic",
        Convert.ToBase64String(
            System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", "kesava", "95123456"))));
            var resp = await client.GetAsync(new Uri("http://dynupdate.no-ip.com/nic/update?hostname=kesava89.ddns.net&myip=" + internetip));
            var respType = resp.EnsureSuccessStatusCode();
            if(respType.IsSuccessStatusCode)
            await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
      () => { statusTb.Text = "Updating Local IP complete."; });
            else
                await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
      () => { statusTb.Text = "Please check your internet connection or contact admin. Code: " + respType.StatusCode; });
        }

        private async void updateIpBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
