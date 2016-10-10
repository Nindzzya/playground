using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calculator
{
    public partial class Form1 : Form
    {
        bool isKeypadHit = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "Hello world";
        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "1";
            isKeypadHit = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "2";
            isKeypadHit = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "3";
            isKeypadHit = true;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "4";
            isKeypadHit = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "5";
            isKeypadHit = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "6";
            isKeypadHit = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "7";
            isKeypadHit = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "8";
            isKeypadHit = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "9";
            isKeypadHit = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "0";
            isKeypadHit = true;
        }
    }
}
