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
        public Dictionary<int, int> SlotMap = new Dictionary<int, int> { { 1, 4 },{ 2, 3 },{ 3, 2 },{ 4, 1 } };
        public Dictionary<int, double[]> CGPAMap = new Dictionary<int, double[]>();
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
            var button = sender as Button;
            FullList = await getCSV();
            FullListing = new ObservableCollection<Student>(await parseCSV());
            setGroups();
            AvailableListing = new ObservableCollection<Student>(FullListing.Where(x=>x.IsAvailable==true));
        }


        public sealed class StudentMap : CsvClassMap<Student>
        {
            public StudentMap()
            {
                Map(x => x.RecordNo);
                Map(x => x.RollNo);
                Map(x => x.Name);
                Map(x => x.CGPA);
                Map(x => x.IsAvailable);
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
            median = (x.Last().CGPA+((x.First().CGPA - x.Last().CGPA)/2));
            int groupCount = 1;
            while (groupCount < 5)
            {
                var listTemp = x.GetRange(0, x.Count<16?x.Count:16);
                CGPAMap.Add(groupCount, new double[] {listTemp[0].CGPA,listTemp.Last().CGPA});
                listTemp.Remove(listTemp.Last());
                foreach(var item in listTemp)
                {
                    FullListing[FullListing.IndexOf(item)].groupId = groupCount;
                    x.Remove(item);
                }
                groupCount++;
            }
        }
        private void QueryBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (QueryBox.Text != "")
            {
                var IEnum = AvailableListing.Where((item) => { return item.Name.ToUpper().Contains(QueryBox.Text.ToUpper()); });
                var list = new List<Student>(IEnum);
                var IEnum1 = AvailableListing.Where(x => { return x.RollNo.Contains(QueryBox.Text); });
                foreach (var item in IEnum1) if (!list.Contains(item)) list.Add(item);
                var IEnum2 = AvailableListing.Where(x => { return x.CGPA.ToString().Contains(QueryBox.Text); });
                foreach (var item in IEnum2) if (!list.Contains(item)) list.Add(item);
                list = new List<Student>(list.OrderByDescending(x => x.CGPA));
                searchListView.ItemsSource = list;
            }
           
        }
        public void refreshSuggestions()
        {
            if (SelectedStudents.Count < 3)
                searchListView.ItemsSource = getBlockSuggestions();
            else if (SelectedStudents.Count == 3) searchListView.ItemsSource = getFinalBlock();
            else if(SelectedStudents.Count == 4) searchListView.ItemsSource = null;
            else searchListView.ItemsSource = null;
        }
        public List<Student> getFinalBlock()
        {
            double sum = 0;
            foreach (var item in SelectedStudents) sum += item.CGPA;
            var expectedCGPA = (CGPALimit * 4) - sum;
            var expectedCGPAMax = (CGPALimit * 4) - sum + 0.6;
            var expectedCGPAMin = (CGPALimit * 4) - sum - 0.6;
            List<Student> finalList = new List<Student>();
            foreach(var item in AvailableListing)
            {
                if (expectedCGPAMax > item.CGPA && expectedCGPAMin <= item.CGPA)
                    finalList.Add(item);
            }
            return finalList;
        }

        public List<Student> getBlockSuggestions()
        {
            int currentBlock = 1 ;
            while(currentBlock<5)
            {
                var cBlockMax = CGPAMap[currentBlock][0];
                var cBlockMin = CGPAMap[currentBlock][1];
                if (currentAvg < cBlockMax && currentAvg >= cBlockMin)
                    return new List<Student>(AvailableListing.Where(item => item.groupId == SlotMap[currentBlock]));
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
            refreshSuggestions();
        }

        private void searchListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchListView.SelectedItem != null)
            {
                SelectedStudents.Add(searchListView.SelectedItem as Student);
                AvailableListing.Remove(searchListView.SelectedItem as Student);
                QueryBox.Text = "";
                ComputeAverage();
                refreshSuggestions();
            }
        }

        public void ComputeAverage()
        {
            double sum =0;
            foreach (var item in SelectedStudents)
                sum += item.CGPA;
            currentAvg = (sum / SelectedStudents.Count);
            averageTxt.Text = currentAvg.ToString();
        }
    }

    public class Student
    {
        public int RecordNo { get; set; }
        public string RollNo { get; set; }
        public string Name { get; set; }
        public double CGPA { get; set; }
        public bool IsAvailable { get; set; }
        public int groupId{get;set;} = -1;
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
