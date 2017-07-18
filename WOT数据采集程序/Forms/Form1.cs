using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using endl = System.Environment;

namespace WOT数据采集程序
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Directory.SetCurrentDirectory(Application.StartupPath);
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            grayTextBox2.ForeColor = SystemColors.WindowText;
            grayTextBox2.Text = Path.GetFullPath(@"Output");
            grayTextBox6.ForeColor = SystemColors.WindowText;
            grayTextBox6.Text = Path.GetFullPath(@"Output");
            grayTextBox8.ForeColor = SystemColors.WindowText;
            grayTextBox8.Text = Path.GetFullPath(@"Output");
            grayTextBox15.ForeColor = SystemColors.WindowText;
            grayTextBox15.Text = Path.GetFullPath(@"Output");
            grayTextBox19.ForeColor = SystemColors.WindowText;
            grayTextBox19.Text = Path.GetFullPath(@"Output");

            TextBoxConsole.SetTextBox(textBox1);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            flags.pythonFlag = CacheProvider.CheckPython();
        }

        private string prewotPath;

        public GetTextProvider getTextProvider = new GetTextProvider("MO");

        private class Flags
        {
            public bool resetFlag = true;
            public bool vehicleFlag = true;
            public bool mapFlag = true;
            public bool achievementFlag = true;
            public bool questFlag = true;
            public bool pythonFlag = false;
        }

        Flags flags = new Flags();

        private void button5_Click(object sender, EventArgs e)//简单模式-开始
        {
            try
            {
                if (grayTextBox1.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择坦克世界安装目录！");
                if (grayTextBox2.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择导出目录！");
            }
            catch (Exception ex)
            {
                TextBoxConsole.WriteLine(endl.NewLine + ex.Message);
                return;
            }

            string wotPath = grayTextBox1.Text;
            string outputPath = grayTextBox2.Text;

            TextBoxConsole.WriteLine(endl.NewLine + "当前坦克世界安装目录：" + wotPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前导出目录：" + outputPath);

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            if (checkedListBox1.CheckedItems.Count != 0)
            {
                if (wotPath != prewotPath || flags.resetFlag)
                {
                    flags.vehicleFlag = true;
                    flags.mapFlag = true;
                    flags.achievementFlag = true;
                    flags.questFlag = true;
                    if (!CacheProvider.GetMOandXml(wotPath))
                        return;
                }

                foreach (object itemChecked in checkedListBox1.CheckedItems)
                {
                    int Index = checkedListBox1.Items.IndexOf(itemChecked);
                    if (Index == 0)
                    {
                        if (flags.vehicleFlag)
                            if (!VehiclesHandler.GetVehicles(getTextProvider))
                                return;
                        flags.vehicleFlag = false;
                        VehiclesHandler.WriteVehicles(comboBox1.SelectedIndex, outputPath, checkBox1.Checked);
                    }
                    else if (Index == 1)
                    {
                        if (flags.mapFlag)
                            if (!MapsHandler.GetMaps(getTextProvider))
                                return;
                        flags.mapFlag = false;
                        MapsHandler.WriteMaps(comboBox1.SelectedIndex, outputPath, checkBox1.Checked);
                    }
                    else if (Index == 2 && flags.pythonFlag)
                    {
                        if (flags.achievementFlag)
                        {
                            if (!AchievementsHandler.GetAchievements(getTextProvider))
                                return;
                        }
                        flags.achievementFlag = false;
                        AchievementsHandler.WriteAchievements(comboBox1.SelectedIndex, outputPath, checkBox1.Checked);
                    }
                    else if (Index == 3)
                    {
                        if (flags.questFlag)
                            if (!PersonalMissionsHandler.GetQuests(getTextProvider))
                                return;
                        flags.questFlag = false;
                        PersonalMissionsHandler.WriteQuests(comboBox1.SelectedIndex, outputPath, checkBox1.Checked);
                    }
                }
                if (wotPath != prewotPath || flags.resetFlag)
                {
                    flags.resetFlag = false;
                    prewotPath = wotPath;
                }
            }

            if (checkedListBox2.CheckedItems.Count != 0)
            {
                if (!ImagesHandler.Ready(wotPath))
                    return;
                foreach (object itemChecked in checkedListBox2.CheckedItems)
                {
                    int Index = checkedListBox2.Items.IndexOf(itemChecked);
                    if (Index == 0)
                    {
                        ImagesHandler.GetAll(outputPath);
                        break;
                    }
                    else if (Index == 1)
                        ImagesHandler.GetIcons(outputPath, "vehicle");
                    else if (Index == 2)
                        ImagesHandler.GetIcons(outputPath, "map");
                    else if (Index == 3)
                        ImagesHandler.GetIcons(outputPath, "achievement");
                    else if (Index == 4)
                        ImagesHandler.GetIcons(outputPath, "marksOnGun");
                    else if (Index == 5)
                        ImagesHandler.GetIcons(outputPath, "tooltip");
                }
            }

            TextBoxConsole.WriteLine(endl.NewLine + "工作结束！");
        }

        private void button6_Click(object sender, EventArgs e)//简单模式-重置缓存
        {
            flags.resetFlag = true;

            TextBoxConsole.WriteLine(endl.NewLine + "缓存已重置！");
        }

        private void button11_Click(object sender, EventArgs e)//高级模式-车辆-添加
        {
            flags.resetFlag = true;
            try
            {
                if (grayTextBox3.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已解密的车辆Xml文件！");
                if (grayTextBox4.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择对应的MO文件！");
                if (grayTextBox5.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择igr_vehicles.mo！");
                if (grayTextBox7.ForeColor == SystemColors.GrayText)
                    throw new Exception("未输入countryid！");
            }
            catch (Exception ex)
            {
                TextBoxConsole.WriteLine(endl.NewLine + ex.Message);
                return;
            }

            string xmlPath = grayTextBox3.Text;
            string moPath = grayTextBox4.Text;
            string igrPath = grayTextBox5.Text;
            int countryid = Int32.Parse(grayTextBox7.Text);

            TextBoxConsole.WriteLine(endl.NewLine + "当前已解密的车辆Xml文件：" + xmlPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前对应的MO文件：" + moPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前igr_vehicles.mo：" + igrPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前countryid：" + countryid + endl.NewLine);

            if (!CacheProvider.GetMO(moPath))
                return;
            if (!CacheProvider.GetMO(igrPath))
                return;

            VehiclesHandler.GetVehiclesXMLFile(xmlPath, countryid, Path.GetFileNameWithoutExtension(moPath), getTextProvider);

            TextBoxConsole.WriteLine(endl.NewLine + "工作结束！");
        }

        private void button12_Click(object sender, EventArgs e)//高级模式-车辆-导出
        {
            if (grayTextBox6.ForeColor == SystemColors.GrayText)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "未选择导出目录！");
                return;
            }

            string outputPath = grayTextBox6.Text;

            TextBoxConsole.WriteLine(endl.NewLine + "当前导出目录：" + outputPath);

            VehiclesHandler.AutoReapir();
            VehiclesHandler.WriteVehicles(comboBox2.SelectedIndex, outputPath, checkBox2.Checked);

            TextBoxConsole.WriteLine(endl.NewLine + "工作结束！");
        }

        private void button13_Click(object sender, EventArgs e)//高级模式-车辆-重置
        {
            flags.vehicleFlag = true;
            VehiclesHandler.Reset();

            TextBoxConsole.WriteLine(endl.NewLine + "车辆数据已重置");
        }

        private void button14_Click(object sender, EventArgs e)//高级模式-地图-导出
        {
            flags.resetFlag = true;
            try
            {
                if (grayTextBox10.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已解密的地图Xml文件！");
                if (grayTextBox9.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择地图MO文件！");
                if (grayTextBox8.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择导出目录！");
            }
            catch (Exception ex)
            {
                TextBoxConsole.WriteLine(endl.NewLine + ex.Message);
                return;
            }
            string xmlPath = grayTextBox10.Text;
            string moPath = grayTextBox9.Text;
            string outputPath = grayTextBox8.Text;

            TextBoxConsole.WriteLine(endl.NewLine + "当前已解密的车辆Xml文件：" + xmlPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前地图MO文件：" + moPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前导出目录：" + outputPath + endl.NewLine);

            if (!CacheProvider.GetMO(moPath))
                return;

            if (!MapsHandler.GetMapsXMLFile(xmlPath, Path.GetFileNameWithoutExtension(moPath), getTextProvider))
                return;

            MapsHandler.WriteMaps(comboBox3.SelectedIndex, outputPath, checkBox3.Checked);

            TextBoxConsole.WriteLine(endl.NewLine + "工作结束！");
        }

        private void button27_Click(object sender, EventArgs e)//高级模式-成就-导出
        {
            flags.resetFlag = true;
            try
            {
                if (grayTextBox16.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已解密的成就Xml文件！");
                if (grayTextBox17.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已反编译的py文件！");
                if (grayTextBox18.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择MO文件！");
                if (grayTextBox19.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择导出目录！");
            }
            catch (Exception ex)
            {
                TextBoxConsole.WriteLine(endl.NewLine + ex.Message);
                return;
            }
            string xmlPath = grayTextBox16.Text;
            string pyPath = grayTextBox17.Text;
            string moPath = grayTextBox18.Text;
            string outputPath = grayTextBox19.Text;

            TextBoxConsole.WriteLine(endl.NewLine + "当前已解密的成就Xml文件：" + xmlPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前已反编译的py文件：" + pyPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前地图MO文件：" + moPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前导出目录：" + outputPath + endl.NewLine);

            if (!CacheProvider.GetMO(moPath))
                return;

            if (!AchievementsHandler.GetAchievementsXMLFile(xmlPath, pyPath, Path.GetFileNameWithoutExtension(moPath), getTextProvider))
                return;

            AchievementsHandler.WriteAchievements(comboBox5.SelectedIndex, outputPath, checkBox5.Checked);

            TextBoxConsole.WriteLine(endl.NewLine + "工作结束！");
        }

        private void button2_Click(object sender, EventArgs e)//高级模式-个人任务-导出
        {
            flags.resetFlag = true;
            try
            {
                if (grayTextBox11.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已解密的seasons.xml！");
                if (grayTextBox12.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已解密的tiles.xml！");
                if (grayTextBox13.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择已解密的list.xml！");
                if (grayTextBox14.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择potapov_quests.mo！");
                if (grayTextBox15.ForeColor == SystemColors.GrayText)
                    throw new Exception("未选择导出目录！");
            }
            catch (Exception ex)
            {
                TextBoxConsole.WriteLine(endl.NewLine + ex.Message);
                return;
            }
            string seasonsPath = grayTextBox11.Text;
            string tilesPath = grayTextBox12.Text;
            string questsPath = grayTextBox13.Text;
            string moPath = grayTextBox14.Text;
            string outputPath = grayTextBox15.Text;

            TextBoxConsole.WriteLine(endl.NewLine + "当前已解密的seasons.xml：" + seasonsPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前已解密的tiles.xml：" + tilesPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前已解密的list.xml：" + questsPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前potapov_quests.mo：" + moPath);
            TextBoxConsole.WriteLine(endl.NewLine + "当前导出目录：" + outputPath + endl.NewLine);

            if (!CacheProvider.GetMO(moPath))
                return;

            if (!PersonalMissionsHandler.GetQuestsXMLFile(seasonsPath, tilesPath, questsPath, Path.GetFileNameWithoutExtension(moPath), getTextProvider))
                return;

            PersonalMissionsHandler.WriteQuests(comboBox4.SelectedIndex, outputPath, checkBox4.Checked);

            TextBoxConsole.WriteLine(endl.NewLine + "工作结束！");
        }
    }
}