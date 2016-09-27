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
        int n1 = 0;
        int temp = 0;
        bool isOperatorHit = false;
        int operatorNode = 0;
        bool pending = false;

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

            isOperatorHit = true;
            if (n1 == 0)
            {
                n1 = int.Parse(textBox1.Text);
                operatorNode = 3;
                return;
            }
            temp = int.Parse(textBox1.Text);
            n1 = Calculate();
            operatorNode = 3;
            textBox1.Text = n1.ToString();

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }

            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "1";
            isKeypadHit = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "2";
            isKeypadHit = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "3";
            isKeypadHit = true;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "4";
            isKeypadHit = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "5";
            isKeypadHit = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "6";
            isKeypadHit = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "7";
            isKeypadHit = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "8";
            isKeypadHit = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "9";
            isKeypadHit = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (isOperatorHit == true)
            {
                textBox1.Text = "";
                isOperatorHit = false;
            }
            if (!isKeypadHit)
                textBox1.Text = "";
            textBox1.Text += "0";
            isKeypadHit = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            isOperatorHit = true;
            if (n1 == 0)
            {
                n1 = int.Parse(textBox1.Text);
                operatorNode = 1;
                return;
            }
            temp = int.Parse(textBox1.Text);
            n1 = Calculate();
            operatorNode = 1;
            textBox1.Text = n1.ToString();
        }

        private void button13_Click(object sender, EventArgs e)
        {

            isOperatorHit = true;
            if (n1 == 0)
            {
                n1 = int.Parse(textBox1.Text);
                operatorNode = 2;
                return;
            }
            temp = int.Parse(textBox1.Text);
            n1 = Calculate();
            operatorNode = 2;
            textBox1.Text = n1.ToString();

        }

        private void button15_Click(object sender, EventArgs e)
        {

            isOperatorHit = true;
            if (n1 == 0)
            {
                n1 = int.Parse(textBox1.Text);
                operatorNode = 4;
                return;
            }
            temp = int.Parse(textBox1.Text);
            n1 = Calculate();
            operatorNode = 4;
            textBox1.Text = n1.ToString();

        }

        public int Calculate()
        {
            switch (operatorNode)
            {
                case 1:
                    return n1 + temp;
                case 2:
                    return n1 - temp;
                case 3:
                    return n1 * temp;
                case 4:
                    return n1 / temp;
            }
            pending = false;
            return 0;
        }

        private void button16_Click(object sender, EventArgs e)
        {
            temp = int.Parse(textBox1.Text);
            n1 = Calculate();
            textBox1.Text = n1.ToString();
            n1 = 0;
        }
    }
}
