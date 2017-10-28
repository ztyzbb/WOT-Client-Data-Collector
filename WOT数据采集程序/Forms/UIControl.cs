using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace WOTDataCollector
{
    public partial class Form1
    {
        private void button3_Click(object sender, EventArgs e)//简单模式-WoT安装目录选择
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox1.ForeColor = SystemColors.WindowText;
                grayTextBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button4_Click(object sender, EventArgs e)//简单模式-导出目录选择
        {
            folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox2.ForeColor = SystemColors.WindowText;
                grayTextBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)//简单模式-导出格式
        {
            if (comboBox1.SelectedIndex == 0)
                checkBox1.Enabled = true;
            else
                checkBox1.Enabled = false;
        }

        private void button7_Click(object sender, EventArgs e)//高级模式-车辆-xml选择
        {
            openFileDialog1.Filter = "已解密的车辆XML文件|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox3.ForeColor = SystemColors.WindowText;
                grayTextBox3.Text = openFileDialog1.FileName;
            }
        }

        private void button8_Click(object sender, EventArgs e)//高级模式-车辆-对应MO选择
        {
            openFileDialog1.Filter = "MO文件|*.mo";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox4.ForeColor = SystemColors.WindowText;
                grayTextBox4.Text = openFileDialog1.FileName;
            }
        }

        private void button9_Click(object sender, EventArgs e)//高级模式-车辆-igr_vehicles.mo选择
        {
            openFileDialog1.Filter = "MO文件|igr_vehicles.mo";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox5.ForeColor = SystemColors.WindowText;
                grayTextBox5.Text = openFileDialog1.FileName;
            }
        }

        private void button10_Click(object sender, EventArgs e)//高级模式-车辆-导出目录选择
        {
            folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox6.ForeColor = SystemColors.WindowText;
                grayTextBox6.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)//高级模式-车辆-导出格式
        {
            if (comboBox2.SelectedIndex == 0)
                checkBox2.Enabled = true;
            else
                checkBox2.Enabled = false;
        }

        private void button17_Click(object sender, EventArgs e)//高级模式-地图-xml选择
        {
            openFileDialog1.Filter = "已解密的地图XML文件|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox10.ForeColor = SystemColors.WindowText;
                grayTextBox10.Text = openFileDialog1.FileName;
            }
        }

        private void button16_Click(object sender, EventArgs e)//高级模式-地图-MO选择
        {
            openFileDialog1.Filter = "MO文件|*.mo";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox9.ForeColor = SystemColors.WindowText;
                grayTextBox9.Text = openFileDialog1.FileName;
            }
        }

        private void button15_Click(object sender, EventArgs e)//高级模式-地图-导出目录选择
        {
            folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox8.ForeColor = SystemColors.WindowText;
                grayTextBox8.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)//高级模式-地图-导出格式
        {
            if (comboBox3.SelectedIndex == 0)
                checkBox3.Enabled = true;
            else
                checkBox3.Enabled = false;
        }

        private void button23_Click(object sender, EventArgs e)//高级模式-成就-xml选择
        {
            openFileDialog1.Filter = "已解密的成就XML文件|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox16.ForeColor = SystemColors.WindowText;
                grayTextBox16.Text = openFileDialog1.FileName;
            }
        }

        private void button24_Click(object sender, EventArgs e)//高级模式-成就-records.py选择
        {
            openFileDialog1.Filter = "已经反编译的py文件|*.py";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox17.ForeColor = SystemColors.WindowText;
                grayTextBox17.Text = openFileDialog1.FileName;
            }
        }

        private void button25_Click(object sender, EventArgs e)//高级模式-成就-achievements.mo选择
        {
            openFileDialog1.Filter = "MO文件|*.mo";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox18.ForeColor = SystemColors.WindowText;
                grayTextBox18.Text = openFileDialog1.FileName;
            }
        }

        private void button26_Click(object sender, EventArgs e)//高级模式-成就-导出目录选择
        {
            folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox19.ForeColor = SystemColors.WindowText;
                grayTextBox19.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)//高级模式-成就-导出格式
        {
            if (comboBox5.SelectedIndex == 0)
                checkBox5.Enabled = true;
            else
                checkBox5.Enabled = false;
        }

        private void button18_Click(object sender, EventArgs e)//高级模式-个人任务-seasons.xml选择
        {
            openFileDialog1.Filter = "已解密的seasons.xml|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox11.ForeColor = SystemColors.WindowText;
                grayTextBox11.Text = openFileDialog1.FileName;
            }
        }

        private void button19_Click(object sender, EventArgs e)//高级模式-个人任务-tiles.xml选择
        {
            openFileDialog1.Filter = "已解密的tiles.xml|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox12.ForeColor = SystemColors.WindowText;
                grayTextBox12.Text = openFileDialog1.FileName;
            }
        }

        private void button20_Click(object sender, EventArgs e)//高级模式-个人任务-list.xml选择
        {
            openFileDialog1.Filter = "已解密的lists.xml|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox13.ForeColor = SystemColors.WindowText;
                grayTextBox13.Text = openFileDialog1.FileName;
            }
        }

        private void button21_Click(object sender, EventArgs e)//高级模式-个人任务-potapov_quests.mo选择
        {
            openFileDialog1.Filter = "potapov_quests.mo|*.mo";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox14.ForeColor = SystemColors.WindowText;
                grayTextBox14.Text = openFileDialog1.FileName;
            }
        }

        private void button22_Click(object sender, EventArgs e)//高级模式-个人任务-导出目录选择
        {
            folderBrowserDialog1.SelectedPath = Directory.GetCurrentDirectory();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                grayTextBox15.ForeColor = SystemColors.WindowText;
                grayTextBox15.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)//高级模式-个人任务-导出格式
        {
            if (comboBox4.SelectedIndex == 0)
                checkBox4.Enabled = true;
            else
                checkBox4.Enabled = false;
        }
    }
}
