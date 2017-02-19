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
using System;
using System.Linq;
using System.Windows;
using Windows.UI.Core;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace testApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static int countElapse = 1;
        public static string currentDate = DateTime.Now.Date.ToString();
        public static string classRoom = "-1";
        public static int[] countS = new int[3];
        public bool isStarted = false;
        public ObservableCollection<StorageFile> fileColl = new ObservableCollection<StorageFile>();
        public static DateTime dateX = DateTime.Now.ToLocalTime().Date;
        public static List<NameRecord> NameList = new List<NameRecord>();
        public static List<studentPresence> students = new List<studentPresence>();
        public MainPage()
        {
            this.InitializeComponent();
            var date = DateTime.Now.ToLocalTime().Date;
            dateTb.Text = String.Format("{0}/{1}/{2}", date.Day, date.Month, date.Year);
            hourTb.Text ="Hour " + getHour().ToString();
        }

        public enum presenceState
        {
            PRESENT,
            ABSENT,
            NOT_AVAILABLE,
            ON_DUTY
        }

        public class NameRecord
        {
            public string rollNo;
            public string name;
            public NameRecord(string rNo, string nme)
            {
                rollNo = rNo;
                name = nme;
            }
        }

        public class studentPresence
        {
            public string studentClass;            
            public string date = String.Format("{0}/{1}/{2}", dateX.Day, dateX.Month, dateX.Year);
            public int hour = getHour();
            public string rollNo;
            public presenceState state;

            public studentPresence(string sClass, string rNo,presenceState st,int hr = -1, string dt = "-1")
            {
                studentClass = sClass;
                rollNo = rNo;                
                state = st;
                if (hr != -1) this.hour = hr;
                if (dt != "-1") this.date = dt;
            }
        }        

        public static int getHour()
        {
            var time = DateTime.Now.ToLocalTime().Hour;
            switch (time)
            {
                case 8:
                    return 1;
                case 9:
                    return 2;
                case 11:
                    return 3;
                case 12:
                    return 4;
                case 14:
                    return 5;
                case 15:
                    return 6;
                case 16:
                    return 7;
                default:
                    return 0;
            }
        }
        
        private async Task decodeCsv(StorageFile file)
        {
            string contents="";
            NameList.Clear();
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                contents = dataReader.ReadString(buffer.Length);
            }
            contents = contents.Replace("\r", "");
            var List = contents.Split('\n');
            var index = 0;
            foreach (var item in List)
            {
                if (++index < List.Count())
                {
                    var x = item.Split(',');
                    NameList.Add(new NameRecord(x[0], x[1]));
                }
            }
        }
        private void updateStatusItems()
        {
            cRollno.Text = NameList[countElapse].rollNo;
            pCount.Text = countS[0].ToString();
            aCount.Text = countS[1].ToString();
            oCount.Text = countS[2].ToString();
        }
        private async Task<bool> checkifend()
        {
            if(countElapse==NameList.Count)
            {
                isStarted = false;
                //classTb.IsEnabled = true;
                presenceControl.Visibility = Visibility.Collapsed;
                await generateCustomCSV();
                countElapse = 1;
                countS[0] = 0;
                countS[1] = 0;
                countS[2] = 0;
                students.Clear();
                appControl.Content = "Start";
                cRollno.Text = "0";
                return true;
            }
            return false;
        }
        private async void presentB_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            switch (button.Name)
            {
                case "presentB":
                    countS[0]++;
                    students.Add(new studentPresence(classRoom, NameList[countElapse].rollNo, presenceState.PRESENT));
                    countElapse++;
                    if (!await checkifend())
                        updateStatusItems();
                    break;
                case "absentB":
                    countS[1]++;
                    students.Add(new studentPresence(classRoom, NameList[countElapse].rollNo, presenceState.ABSENT));
                    countElapse++;
                    if (!await checkifend())
                        updateStatusItems();
                    break;
                case "odB":
                    countS[2]++;
                    students.Add(new studentPresence(classRoom, NameList[countElapse].rollNo, presenceState.ON_DUTY));
                    countElapse++;
                    if (!await checkifend())
                        updateStatusItems();
                    break;
                case "naB":
                    countS[0]++;
                    students.Add(new studentPresence(classRoom, NameList[countElapse].rollNo, presenceState.NOT_AVAILABLE));
                    countElapse++;
                    if (!await checkifend())
                        updateStatusItems();
                    break;
                default:
                    break;
            }
        }

        private async void appControl_Click(object sender, RoutedEventArgs e)
        {
            if (selectClass.Content.ToString() == "Select Class")
            {
                await new MessageDialog("Please Set Classroom to Continue").ShowAsync();
                return;
            }
            if (isStarted == false)
            {
                isStarted = true;
                presenceControl.Visibility = Visibility.Visible;
                appControl.Content = "Done";
                classRoom = selectClass.Content.ToString().ToUpper();
                //classTb.IsEnabled = false;
                cRollno.Text = NameList[countElapse].rollNo;
            }
            else
            {
                isStarted = false;
                //classTb.IsEnabled = true;
                presenceControl.Visibility = Visibility.Collapsed;
                generateCustomCSV();
                appControl.Content = "Start";
                cRollno.Text = "0";
            }
        }

        private void undoB_Click(object sender, RoutedEventArgs e)
        {
            switch(students[students.Count-1].state)
            {
                case presenceState.PRESENT:
                    countS[0]--;
                    break;
                case presenceState.ABSENT:
                    countS[1]--;
                    break;
                case presenceState.ON_DUTY:
                    countS[2]--;
                    break;
                default:
                    break;
            }
            countElapse--;
            students.Remove(students.Last());
            updateStatusItems();
        }
        private async Task generateCSV()
        {
            List<string> records = new List<string>();
            string output = "";
            output = "class,rollno";
            foreach (var item in students)
            {
                output = output + string.Format("\n{0},{1},{2},{3},{4}", classRoom, item.date, item.hour, item.rollNo, item.state);
            }
            await new MessageDialog(output).ShowAsync();
        }

        private async Task generateCustomCSV()
        {
            string toWrite="";
            StorageFolder storageFolder;
            try
            {
                storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("results");
            }
            catch
            {
                storageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("results");
            }
            StorageFile file = await storageFolder.CreateFileAsync(selectClass.Content.ToString()+".csv",CreationCollisionOption.OpenIfExists);
            string contents;
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                contents = dataReader.ReadString(buffer.Length);
            }
            if (contents != "")
            {
                contents = contents.Replace("\r", "");
                var RecordsList = contents.Split('\n');
                RecordsList[0]+=","+students[0].date + "-" + students[0].hour;
                var index = 1;
                foreach (var item in students)
                {
                    RecordsList[index++] += ","+item.state;
                }
                var output = RecordsList[0];
                var outputList = RecordsList.Skip(1);
                foreach(var item in outputList)
                {
                    output += "\n" + item;
                }
                toWrite = output;
            }
            else
            {
                var output = "class,rollno," + students[0].date +"-" + students[0].hour;
                foreach (var item in students)
                {
                    output += string.Format("\n{0},{1},{2}", classRoom, item.rollNo, item.state);
                    toWrite = output;
                }
                await new MessageDialog(output).ShowAsync();
            }
            await Windows.Storage.FileIO.WriteTextAsync(file,toWrite);
            toWrite = "";
        }
        private async void selectClass_Click(object sender, RoutedEventArgs e)
        {
            fileSelectBorder.Visibility = Visibility.Visible;
            await populateList();
        }

        private async void filePickerBtn_Click(object sender, RoutedEventArgs e)
        {
            fileColl.Clear();
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".csv");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                
               StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
               StorageFile sampleFile = await storageFolder.CreateFileAsync(file.Name, CreationCollisionOption.ReplaceExisting);
               sampleFile =await storageFolder.GetFileAsync(file.Name);
               await Windows.Storage.FileIO.WriteTextAsync(sampleFile, await getContents(file));
               populateList();
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }

        private async Task<string> getContents(StorageFile file)
        {
            string contents = "";
            var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
            using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
            {
                contents = dataReader.ReadString(buffer.Length);
            }
            return contents;
        }

        private async Task populateList()
        {
            fileColl.Clear();
            StorageFolder folders = ApplicationData.Current.LocalFolder;
            IReadOnlyList<StorageFile> folderList =await folders.GetFilesAsync();
            foreach(var item in folderList)
            {
                fileColl.Add(item);
            }           
        }

        private async void fileListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = (StorageFile)fileListView.SelectedItem;
                await decodeCsv(file);
                fileSelectBorder.Visibility = Visibility.Collapsed;
                selectClass.Content = classRoom = file.Name.Replace(".csv", "");
            }
            catch { }
        }


        private async void SlidableListItem_RightCommandRequested(object sender, EventArgs e)
        {
            StorageFolder storageFolder =  ApplicationData.Current.LocalFolder;
            await ((StorageFile)((SlidableListItem)sender).DataContext).DeleteAsync();
            storageFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("results");
            try
            {
                var file = await storageFolder.GetFileAsync(((StorageFile)((SlidableListItem)sender).DataContext).Name);
                await file.DeleteAsync();
            }
            catch { }
            var x = fileColl.Remove((StorageFile)((SlidableListItem)sender).DataContext);
        }
    }

}
