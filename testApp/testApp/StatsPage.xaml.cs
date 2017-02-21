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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace testApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatsPage : Page
    {
        public string classRoom;
        private int pNo = 0;
        private int aNo = 0;
        private int tNo = 0;
        public string[] List;
        public DateTime currentDate;
        public string csvContents;
        public StatsPage()
        {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var x = (Tuple<string, string>)e.Parameter;
            classRoom = x.Item1;
            csvContents = x.Item2;
            getValidDates();
        }

        List<string> dateHList = new List<string>();
        List<string> dateList = new List<string>();
        List<string> hourList = new List<string>();

        private void getValidDates()
        {

            var contents = csvContents.Replace("\r", "");
            List = contents.Split('\n');
            var header = List[0];
            var dateSplit = header.Split(',');
            int index = 1;
            dateHList.AddRange(dateSplit.Skip(2)); 
            foreach(var item in dateHList)
            {
                var x = item.Split('-');
                hourList.Add(x[1]);
                dateList.Add(x[0]);
            }
        }

        private void datePicker_CalendarViewDayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            var x = dateList.Find(
                delegate (string dt)
                {
                    return dt == String.Format("{0}/{1}/{2}",args.Item.Date.Day, args.Item.Date.Month, args.Item.Date.Year);
                });
            if(x==null)
            args.Item.IsBlackout = true;
        }

        private void datePicker_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            datePicker.Visibility = Visibility.Collapsed;
            fadeoutBorder.Visibility = Visibility.Collapsed;
            currentDate = datePicker.SelectedDates[0].Date;
            gatherStats(currentDate);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            datePicker.Visibility = Visibility.Visible;
            fadeoutBorder.Visibility = Visibility.Visible;
        }

        private void updateUI()
        {
            pBar.Maximum = tNo;
            pBar.Value = pNo;
            pTb.Text = pNo.ToString();
            aTb.Text = aNo.ToString();
        }

        private void gatherStats(DateTime Date)
        {
            string dt = String.Format("{0}/{1}/{2}", Date.Day, Date.Month, Date.Year);
            tNo = List.Count() - 1;
            int index = 0;            
            var headSplit = List[0].Split(',');
            foreach (var item in headSplit)
                if (item.Split('-')[0] != dt)
                    index++;
            var presentS = MainPage.presenceState.PRESENT.ToString();
            var absentS = MainPage.presenceState.ABSENT.ToString();
            
                foreach(var item in List)
                {
                    var split = item.Split(',');
                    if (split[index].Split('-')[0] == presentS)
                        pNo++;
                    else if (split[index].Split('-')[0] == absentS)
                        aNo++;
                }
                updateUI();
            
        }
    }
}
