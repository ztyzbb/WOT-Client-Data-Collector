using System;
using System.Windows.Forms;

namespace WOTDataCollector
{
    public partial class Form2 : Form
    {
        public Form2(string title,string text)
        {
            InitializeComponent();
            Text = title;
            label1.Text = text;
        }
        public string returnvalue;

        private void button1_Click(object sender, EventArgs e)
        {
            returnvalue = textBox1.Text;
            Close();
        }
    }
}
