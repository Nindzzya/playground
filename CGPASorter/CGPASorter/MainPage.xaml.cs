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
        public StorageFile FullList;
        public StorageFile AvailableList;
        public StorageFile UnavailableList;
        public ObservableCollection<Student> FullListing = new ObservableCollection<Student>();
        public ObservableCollection<Student> AvailableListing = new ObservableCollection<Student>();
        public ObservableCollection<Student> UnavailableListing = new ObservableCollection<Student>();
        public ObservableCollection<Student> SelectedStudents = new ObservableCollection<Student>();

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
            switch(button.Content)
            {
                case "Open Full List":
                    FullList = await getCSV();
                    FullListing = new ObservableCollection<Student>(await parseCSV());
                    break;
                case "Open Available List":
                    AvailableList = await getCSV();
                    AvailableListing = new ObservableCollection<Student>(await parseCSV());
                    break;
                case "Open Unavailable List":
                    UnavailableList = await getCSV();
                    UnavailableListing = new ObservableCollection<Student>(await parseCSV());
                    break;
            }
        }

        public class Student
        {
            public int RecordNo { get; set; }
            public string RollNo { get; set; }
            public string Name { get; set; }
            public float CGPA { get; set; }
            public bool IsAvailable { get; set; }
            public void Add(string r, string n, float c)
            {
                RollNo = r;
                Name = n;
                CGPA = c;
            }
        }

        public sealed class StudentMap: CsvClassMap<Student>
        {
            public StudentMap()
            {
                Map(x => x.RollNo);
                Map(x => x.Name);
                Map(x => x.CGPA);
            }
        }
    }
}
