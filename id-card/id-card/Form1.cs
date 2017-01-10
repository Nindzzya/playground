using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using ps = Photoshop;

namespace id_card
{
    public partial class Form1 : Form
    {
        ps.Application app = new ps.Application();
        string csvPath = string.Empty;
        string imagePath = string.Empty;
        string outputPath = string.Empty;
        string TemplatePath = string.Empty;
        recordEntry currentEntry;
        int currentEntryN = 0;
        int count = 0;
        int done = 0;
        public Form1()
        {
            InitializeComponent();
            
        }
        List<recordEntry> entries = new List<recordEntry>();

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        const int MYACTION_HOTKEY_ID = 1;

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Ready";
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID, 2,(int)Keys.Space);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312 && m.WParam.ToInt32() == MYACTION_HOTKEY_ID)
            {
                var saveOptions = new ps.JPEGSaveOptions();
                saveOptions.EmbedColorProfile = true;
                saveOptions.FormatOptions = ps.PsFormatOptionsType.psStandardBaseline;
                saveOptions.Matte = ps.PsMatteType.psNoMatte;
                saveOptions.Quality = 10;
                app.ActiveDocument.SaveAs(outputPath + "\\" + currentEntry.Id + ".jpg", saveOptions, true);
                if (currentEntryN == entries.Count-1)
                {
                    UnregisterHotKey(this.Handle, MYACTION_HOTKEY_ID);
                    closeAll();
                    app.Quit();
                    MessageBox.Show("All Done!");
                }
                else
                {
                    currentEntryN++;
                    openNext();
                }
            }
            base.WndProc(ref m);
        }

        private async 
        
        Task       
doIt(recordEntry entry)
        {
            toolStripStatusLabel1.Text = done + " done of " + count + " - current entry: " + entry.Name;
            //foreach (ps.Document item in app.Documents)
            //item.Close();
            //app.Open(@"C:\Users\Kesava for VS\Documents\Untitled-1.psd");           
            //app.ActiveDocument.ArtLayers.Add();    
            closeAll();
            switch(entry.Desg)
            {
                case "coordinator":
                    app.Open(TemplatePath+"\\coordinator.psd");
                    break;
                case "volunteer":
                    app.Open(TemplatePath + "\\volunteer.psd");
                    break;
                default:
                    MessageBox.Show("Undefined Designation. Please check csv. Errors are emminent.", "Error", MessageBoxButtons.OK);
                    break;
            }
            foreach(ps.ArtLayer item in app.ActiveDocument.Layers)
            {
                if (item.Kind == ps.PsLayerKind.psTextLayer)
                {
                    item.TextItem.Contents = entry.Name;
                }
            }
            app.Open(imagePath+"\\" + entry.Id);
            var y = app.Documents;
            app.ActiveDocument = app.Documents[2];
            var test = app.ActiveDocument.Name;
            app.ActiveDocument.ArtLayers[1].Copy();
            app.ActiveDocument = app.Documents[1];
            app.ActiveDocument.ActiveLayer = app.ActiveDocument.ArtLayers[app.ActiveDocument.ArtLayers.Count];
            app.ActiveDocument.Paste();
            //Add JPG in future
            //var saveOptions = new ps.JPEGSaveOptions();
            //saveOptions.EmbedColorProfile = true;
            //saveOptions.FormatOptions = ps.PsFormatOptionsType.psStandardBaseline;
            //saveOptions.Matte = ps.PsMatteType.psNoMatte;
            //saveOptions.Quality = 10;
            app.ActiveDocument.SaveAs(outputPath +"\\"+ entry.Id + ".psd", new ps.PhotoshopSaveOptions(), true);
            toolStripProgressBar1.Value++;
            done++;
            
        }       
        public void closeAll()
        {
            int count = 0;
            while (app.Documents.Count != 0)
            {
                app.Documents[1].Close(ps.PsSaveOptions.psDoNotSaveChanges);
                count++;
            }
        }
        public async Task saveAsJPG(recordEntry entry)
        {
            toolStripStatusLabel1.Text = done + " done of " + count + " - current entry: " + entry.Name;
            closeAll();
            openFile(entry);
            var saveOptions = new ps.JPEGSaveOptions();
            saveOptions.EmbedColorProfile = true;
            saveOptions.FormatOptions = ps.PsFormatOptionsType.psStandardBaseline;
            saveOptions.Matte = ps.PsMatteType.psNoMatte;
            saveOptions.Quality = 10;
            app.ActiveDocument.SaveAs(outputPath + "\\" + entry.Id + ".jpg", saveOptions, true);
            toolStripProgressBar1.Value++;
            done++;
            
        }
        public class recordEntry
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Desg { get; set; }
            public string Dept { get; set; }
            public recordEntry(string id, string name, string desg, string dept)
            {
                Id = id;
                Name = name;
                Desg = desg;
                Dept = dept;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Comma Separated Values (*.csv)|*.csv";
            if(openFileDialog1.ShowDialog()== DialogResult.OK)
            csvPath = openFileDialog1.FileName;
            textBox1.Text = csvPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var opener = new FolderBrowserDialog();
            if (opener.ShowDialog() == DialogResult.OK)
                imagePath = opener.SelectedPath;
            textBox2.Text = imagePath;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var opener = new FolderBrowserDialog();
            if (opener.ShowDialog() == DialogResult.OK)
                outputPath = opener.SelectedPath;
            textBox3.Text = outputPath;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var opener = new FolderBrowserDialog();
            if (opener.ShowDialog() == DialogResult.OK)
                TemplatePath = opener.SelectedPath;
            textBox4.Text = TemplatePath;
        }

        private async void button5_Click(object sender, EventArgs e)
        {

            foreach (string line in System.IO.File.ReadLines(csvPath))
            {
                var ss = line.Split(',');
                entries.Add(new recordEntry(ss[0], ss[1], ss[2], ss[3]));
            }
            toolStripStatusLabel1.Text = "Initializing...";
            count = entries.Count;
            done = 0;
            toolStripStatusLabel1.Text = done + " done of " + count;
            toolStripProgressBar1.Maximum = count + 1;
            toolStripProgressBar1.Minimum = 1;
            toolStripProgressBar1.Value = 1;
            foreach (var item in entries)
            {
                doIt(item);
            }
            toolStripStatusLabel1.Text = "Finished.";
        }
        private void openFile(recordEntry entry)
        {
            app.Open(outputPath + "\\" + entry.Id + ".psd");
        }
        private void button6_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Go to Photoshop. When you're done with the cropping, press Ctrl+Space to navigate to the next image.","Alert",MessageBoxButtons.OK);
            closeAll();
            entries.Clear();
            foreach (string line in System.IO.File.ReadLines(csvPath))
            {
                var ss = line.Split(',');
                entries.Add(new recordEntry(ss[0], ss[1], ss[2], ss[3]));
            }
            currentEntryN = 0;
            currentEntry = entries[currentEntryN];
            openFile(entries[currentEntryN]);
            //toolStripStatusLabel1.Text = "Initializing...";
            //count = entries.Count;
            //done = 0;
            //toolStripStatusLabel1.Text = done + " done of " + count;
            //toolStripProgressBar1.Maximum = count + 1;
            //toolStripProgressBar1.Minimum = 1;
            //toolStripProgressBar1.Value = 1;
            //foreach (var item in entries)
            //{
            //    saveAsJPG(item);
            //}
        }
        private void openNext()
        {
            currentEntry = entries[currentEntryN];
            openFile(entries[currentEntryN]);
        }
    }
}
