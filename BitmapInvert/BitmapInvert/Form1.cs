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
        Bitmap bitmapImg;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Bitmap";
                dlg.Filter = "bmp files (*.bmp)|*.bmp";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    bitmapImg = new Bitmap(dlg.FileName); //this is the bitmap that you need to manipulate.
                    pictureBox1.Image = new Bitmap(dlg.FileName);
                }
            }
        }


        public Bitmap InvertBitmap(Bitmap Img)
        {
            byte Alpha, Red, Green, Blue;
            Color Pixel;
            for(int j=0;j<Img.Height;j++)
                for(int i=0;i<Img.Width;i++)
                {
                    Pixel = Img.GetPixel(i, j);
                    Alpha = Pixel.A;
                    Red = (Byte)(255 - Pixel.R);
                    Blue = (Byte)(255 - Pixel.B);
                    Green = (Byte)(255 - Pixel.G);
                    Img.SetPixel(i, j, Color.FromArgb((int)Alpha, (int)Red, (int)Green, (int)Blue));
                }
            return Img;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var invertedImg = InvertBitmap(bitmapImg);
            bitmapImg = invertedImg;
            pictureBox1.Image = new Bitmap(invertedImg);
        }
    }
}
