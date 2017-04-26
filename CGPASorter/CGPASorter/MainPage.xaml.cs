using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CGPASorter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public StorageFile FullList = null;
        public StorageFile AvailableList;
        public StorageFile UnavailableList;
        public ObservableCollection<Student> FullListing = new ObservableCollection<Student>();
        public ObservableCollection<Student> AvailableListing = new ObservableCollection<Student>();
        public ObservableCollection<Student> SearchListing = new ObservableCollection<Student>();
        public ObservableCollection<Student> SelectedStudents = new ObservableCollection<Student>();
        public ObservableCollection<Student> SuggestedStudents = new ObservableCollection<Student>();
        public Dictionary<int, int> SlotMap = new Dictionary<int, int> { { 1, 4 }, { 2, 3 }, { 3, 2 }, { 4, 1 } };
        public Dictionary<int, double[]> CGPAMap = new Dictionary<int, double[]>();
        public bool isOpened = false;
        public bool SingleSearch = false;
        public int currentSection = -1;
        public double currentAvg = 0.0;
        public double CGPALimit = 6.5;
        public MainPage()
        {
            this.InitializeComponent();
        }

        public async Task<StorageFile> getCSV()
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".csv");
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
            return await picker.PickSingleFileAsync();
        }

        public async Task<IEnumerable<Student>> parseCSV()
        {
            var streamReader = new StreamReader(await FullList.OpenStreamForReadAsync());
            var parser = new CsvReader(streamReader);
            parser.Configuration.HasHeaderRecord = true;
            parser.Configuration.RegisterClassMap<StudentMap>();
            return parser.GetRecords<Student>();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FullList = await getCSV();
            FullListing = new ObservableCollection<Student>(await parseCSV());
            setGroups();
            AssignSection();
            AvailableListing = new ObservableCollection<Student>(FullListing.Where(x => x.IsAvailable == true));
            searchListView.IsEnabled = true;
            toggleSection.IsEnabled = true;
            isOpened = true;
        }
        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FullList = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"Assets\FullList.csv");
            FullListing = new ObservableCollection<Student>(await parseCSV());
            setGroups();
            AssignSection();
            AvailableListing = new ObservableCollection<Student>(FullListing.Where(x => x.IsAvailable == true));
            searchListView.IsEnabled = true;
            toggleSection.IsEnabled = true;
            isOpened = true;
        }

        public sealed class StudentMap : CsvClassMap<Student>
        {
            public StudentMap()
            {
                Map(x => x.RecordNo);
                Map(x => x.RollNo);
                Map(x => x.Name);
                Map(x => x.CGPA);
            }
        }

        public void AssignSection()
        {
            foreach(var item in FullListing)
            {
                var roll = item.RollNo;
                item.section =int.Parse(roll[13].ToString());
            }
        }

        public void SuggestList()
        {
            double AverageCurrent = double.Parse(averageTxt.Text);
        }

        public void setGroups()
        {
            double median = 0;
            var x = new List<Student>(FullListing.OrderByDescending(item => item.CGPA));
            median = (x.Last().CGPA + ((x.First().CGPA - x.Last().CGPA) / 2));
            var medianH = median + ((x.First().CGPA - median) / 2);
            var medianL = x.Last().CGPA + ((median - x.Last().CGPA) / 2);
            CGPAMap.Add(1, new double[] { x.First().CGPA, medianH });
            CGPAMap.Add(2, new double[] { medianH, median });
            CGPAMap.Add(3, new double[] { median, medianL });
            CGPAMap.Add(4, new double[] { medianL, x.Last().CGPA });
            foreach(var item in FullListing)
            {
                if(item.CGPA>median)
                {
                    if (item.CGPA >= medianH) item.groupId = 1;
                    else item.groupId = 2;
                }
                else
                {
                    if (item.CGPA >= medianL) item.groupId = 3;
                    else item.groupId = 4;
                }
            }          
        }

        public void refreshSuggestions()
        {
                if (SelectedStudents.Count < 3)
                    searchListView.ItemsSource = getBlockSuggestions();
                else if (SelectedStudents.Count == 3) searchListView.ItemsSource = getFinalBlock();
                else if (SelectedStudents.Count == 4) searchListView.ItemsSource = null;
                else searchListView.ItemsSource = null;
                if (searchListView.ItemsSource != null)
                    searchListView.IsSuggestionListOpen = true;
        }
        public List<Student> getFinalBlock()
        {
            double sum = 0;
            foreach (var item in SelectedStudents) sum += item.CGPA;
            var expectedCGPA = (CGPALimit * 4) - sum;
            var expectedCGPAMax = (CGPALimit * 4) - sum + 0.3;
            var expectedCGPAMin = (CGPALimit * 4) - sum - 0.3;
            List<Student> finalList = new List<Student>();
            if(!SingleSearch && currentSection!=-1)
            foreach (var item in AvailableListing)
            {
                if (expectedCGPAMax > item.CGPA && expectedCGPAMin <= item.CGPA)
                    finalList.Add(item);
            }
            else
            {
                foreach (var item in AvailableListing)
                {
                    if (expectedCGPAMax > item.CGPA && expectedCGPAMin <= item.CGPA&&item.section==currentSection)
                        finalList.Add(item);
                }
            }
            return finalList;
        }

        public List<Student> getBlockSuggestions()
        {
            int currentBlock = 1;
            while (currentBlock < 5)
            {
                var cBlockMax = CGPAMap[currentBlock][0] + 0.01;
                var cBlockMin = CGPAMap[currentBlock][1];
                if (!SingleSearch && currentSection != -1)
                {
                    if (currentAvg < cBlockMax && currentAvg >= cBlockMin)
                        return new List<Student>(AvailableListing.Where(item => item.groupId == SlotMap[currentBlock]));
                }
                else
                {
                    if (currentAvg < cBlockMax && currentAvg >= cBlockMin)
                        return new List<Student>(AvailableListing.Where(item => (item.groupId == SlotMap[currentBlock])&& item.section==currentSection));
                }
                currentBlock++;
            }
            return new List<Student>();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectedStudents.Remove(((Button)sender).DataContext as Student);
            SelectionView.ItemsSource = SelectedStudents;
            AvailableListing.Add(((Button)sender).DataContext as Student);
            AvailableListing = new ObservableCollection<Student>(AvailableListing.OrderByDescending(x => x.CGPA));
            ComputeAverage();
        }

        public void ComputeAverage()
        {
            double sum = 0;
            foreach (var item in SelectedStudents)
                sum += item.CGPA;
            currentAvg = (sum / SelectedStudents.Count);
            currentAvg = Math.Round(currentAvg, 3);
            averageTxt.Text = currentAvg.ToString();
        }

        private void searchListView_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            if (args.SelectedItem != null)
            {
                SelectedStudents.Add(args.SelectedItem as Student);
                AvailableListing.Remove(args.SelectedItem as Student);
                ComputeAverage();
                refreshSuggestions();
            }
        }

        private void searchListView_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (searchListView.Text == "CGPASorter.Student")
                searchListView.Text = "";
            if (searchListView.Text != "")
            {
                ObservableCollection<Student> AvailableListing;
                if (SingleSearch && currentSection!=-1) AvailableListing = new ObservableCollection<Student>(this.AvailableListing.Where(x => { return x.section == currentSection; }));
                else AvailableListing = new ObservableCollection<Student>(this.AvailableListing);
                var IEnum = AvailableListing.Where((item) => { return item.Name.ToUpper().Contains(searchListView.Text.ToUpper()); });
                var list = new List<Student>(IEnum);
                var IEnum1 = AvailableListing.Where(x => { return x.RollNo.Contains(searchListView.Text); });
                foreach (var item in IEnum1) if (!list.Contains(item)) list.Add(item);
                var IEnum2 = AvailableListing.Where(x => { return x.CGPA.ToString().Contains(searchListView.Text); });
                foreach (var item in IEnum2) if (!list.Contains(item)) list.Add(item);
                list = new List<Student>(list.OrderByDescending(x => x.CGPA));
                searchListView.ItemsSource = list;
            }
            else refreshSuggestions();
        }

        private void searchListView_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {
            refreshSuggestions();
        }


        private void searchListView_GotFocus(object sender, RoutedEventArgs e)
        {
            refreshSuggestions();
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var success = double.TryParse(CGPABox.Text, out CGPALimit);
            if (!success)
            {
                await (new Windows.UI.Popups.MessageDialog("Please re-check CGPA values \n Default value (6.5) has been set")).ShowAsync();
                CGPALimit = 6.5;
            }
        }



        private void SelectionView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (SelectionView.Items.Count == 1)
                currentSection = (SelectionView.Items.First() as Student).section;
            if (SelectionView.Items.Count == 0)
                currentSection = -1;
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            SingleSearch = ((ToggleSwitch)sender).IsOn;
            //if(SingleSearch)
            //{
            //    foreach (var item in SelectedStudents)
            //        if (item.section != currentSection)
            //            SelectedStudents.Remove(item);
            //}
            refreshSuggestions();
        }
    }
    public class Student
    {
        public int RecordNo { get; set; }
        public string RollNo { get; set; }
        public string Name { get; set; }
        public double CGPA { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int groupId{get;set;} = -1;
        public int section { get; set; } = -1;
        public void Add(string rollNo, string name, float cgpa, int recordNo, bool isAvailable)
        {
            RollNo = rollNo;
            Name = name;
            CGPA = cgpa;
            RecordNo = recordNo;
            IsAvailable = isAvailable;
        }
    }
}
