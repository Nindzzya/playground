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
            double AverageCurrent = double.parse(averageTxt.Text);

        }

        public void setGroups
        {
            double median = 0;
            var x = new List<Student>(FullListing.OrderByDescending(x=>x.CGPA));
            median = (x.Last().CGPA+((x.First().CGPA - x.Last().CGPA)/2));
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
            else
            {
                searchListView.ItemsSource = null;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SelectedStudents.Remove(((Button)sender).DataContext as Student);
            SelectionView.ItemsSource = SelectedStudents;
            AvailableListing.Add(((Button)sender).DataContext as Student);
            ComputeAverage();
        }

        private void searchListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (searchListView.SelectedItem != null)
            {
                SelectedStudents.Add(searchListView.SelectedItem as Student);
                AvailableListing.Remove(searchListView.SelectedItem as Student);
                QueryBox.Text = "";
                ComputeAverage();
            }
        }

        public void ComputeAverage()
        {
            double sum =0;
            foreach (var item in SelectedStudents)
                sum += item.CGPA;
            averageTxt.Text = (sum / SelectedStudents.Count).ToString();
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
