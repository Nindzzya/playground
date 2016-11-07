using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BitmapInvert
{
    public partial class Form1 : Form
    {
        private object originalBmp;

        public object X { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Bitmap";
                dlg.Filter = "bmp files (*.bmp)|*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Bitmap x = new Bitmap(dlg.FileName); //this is the bitmap that you need to manipulate.
                    pictureBox1.Image = new Bitmap(dlg.FileName);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap invertedBmp = null;
            invertedBmp = new Bitmap(originalBmp.width, originalBmp.height);


        
         
        }
    }
}
