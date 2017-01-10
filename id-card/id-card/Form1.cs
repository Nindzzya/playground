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
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Ready";
        }

        
        private async 
        
        Task       
doIt(recordEntry entry)
        {

            //foreach (ps.Document item in app.Documents)
            //item.Close();
            //app.Open(@"C:\Users\Kesava for VS\Documents\Untitled-1.psd");           
            //app.ActiveDocument.ArtLayers.Add();    
            int count=0;
               while(app.Documents.Count!=0)
            {
                app.Documents[1].Close(ps.PsSaveOptions.psDoNotSaveChanges);
                count++;
            }
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
            //var saveOptions = new ps.JPEGSaveOptions();
            //saveOptions.EmbedColorProfile = true;
            //saveOptions.FormatOptions = ps.PsFormatOptionsType.psStandardBaseline;
            //saveOptions.Matte = ps.PsMatteType.psNoMatte;
            //saveOptions.Quality = 10;
            app.ActiveDocument.SaveAs(outputPath +"\\"+ entry.Id + ".psd", new ps.PhotoshopSaveOptions(), true);
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
            List<recordEntry> entries = new List<recordEntry>();
            foreach (string line in System.IO.File.ReadLines(csvPath))
            {
                var ss = line.Split(',');
                entries.Add(new recordEntry(ss[0], ss[1], ss[2], ss[3]));
            }
            toolStripStatusLabel1.Text = "Initializing...";
            int count = entries.Count;
            int done = 0;
            toolStripStatusLabel1.Text = done + " done of " + count;
            toolStripProgressBar1.Maximum = count + 1;
            toolStripProgressBar1.Minimum = 1;
            toolStripProgressBar1.Value = 1;
            foreach (var item in entries)
            {
                await doIt(item);
                toolStripProgressBar1.Value++;
                done++;
                toolStripStatusLabel1.Text = done + " done of " + count;
            }
        }
    }
}
