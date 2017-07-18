using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using wottoolslib;
using System.Xml;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using endl = System.Environment;

namespace WOT数据采集程序
{
    public class VehiclesHandler
    {
        private class Vehicles
        {
            public int id;
            public int countryid;
            public string title;
            public string loacl_name;
            public string short_name;
            public string descr;
            public string icon_orig;
            public int tier;
            public int type;
        }

        private static List<Vehicles> data = new List<Vehicles>();

        private static int errorCounter = 0;

        private static string currentTEXTDOMAIN = null;

        public static bool GetVehicles(GetTextProvider getTextProvider)
        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出车辆数据……");

            XmlDocument xmlreader = new XmlDocument();
            XmlDecompiler xmlDecompiler = XmlDecompiler.Instance;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            int countryid = -1;

            string[] subDir;

            try
            {
                subDir = Directory.GetDirectories(@"encryptedXmls\scripts\item_defs\vehicles");
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }

            foreach (string countryPath in subDir)
            {
                string countryName = Path.GetFileNameWithoutExtension(countryPath);

                if (countryName == "ussr")
                {
                    countryid = 0;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "ussr_vehicles";
                }
                else if (countryName == "germany")
                {
                    countryid = 1;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "germany_vehicles";
                }
                else if (countryName == "usa")
                {
                    countryid = 2;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "usa_vehicles";
                }
                else if (countryName == "china")
                {
                    countryid = 3;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "china_vehicles";
                }
                else if (countryName == "france")
                {
                    countryid = 4;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "france_vehicles";
                }
                else if (countryName == "uk")
                {
                    countryid = 5;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "gb_vehicles";
                }
                else if (countryName == "japan")
                {
                    countryid = 6;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "japan_vehicles";
                }
                else if (countryName == "czech")
                {
                    countryid = 7;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "czech_vehicles";
                }
                else if (countryName == "sweden")
                {
                    countryid = 8;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "sweden_vehicles";
                }
                else if (countryName == "poland")
                {
                    countryid = 9;
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = "poland_vehicles";
                }
                else if (countryName == "common")
                {
                    continue;
                }
                else
                {
                    Form2 Dialog = new Form2("检测到未知的国家！", "当前文件夹:" + countryPath + endl.NewLine + "如果该文件夹中有用于记录对应国家的车辆数据的list.xml，请输入对应的国家代码" + endl.NewLine + "如果没有，请输入负数" + endl.NewLine + "当前已知国家代码：" + endl.NewLine + "0-ussr  1-germany  2-usa  3-china  4-france" + endl.NewLine + "5-gb  6-japan  7-czech  8-sweden 9-poland" + endl.NewLine + "未知国家代码以科技树在游戏中的出现顺序向后递增");
                    Dialog.ShowDialog();
                    countryid = Int32.Parse(Dialog.returnvalue);
                    if (countryid < 0)
                        continue;
                    Dialog = new Form2("请输入该国家对应的*_vehicles.mo文件的名称！", "请输入该国家对应的*_vehicles.mo文件的名称！" + endl.NewLine + "当前文件夹:" + countryName + endl.NewLine + "例如美系MO文件的名称为usa_vehicles.mo，则在下方文本框中输入usa_vehicles" + endl.NewLine + "具体名称请到" + Path.GetFullPath(@"MO\zh_CN\LC_MESSAGES") + "中确认");
                    Dialog.ShowDialog();
                    currentTEXTDOMAIN = getTextProvider.TEXTDOMAIN = Dialog.returnvalue;
                }
                TextBoxConsole.WriteLine(string.Format(endl.NewLine + "当前文件夹:{0,-10}countryid={1}" + endl.NewLine, countryName,countryid));

                try
                {
                    xmlreader.LoadXml(xmlDecompiler.GetFileXml(countryPath + @"\list.xml"));

                    using (XmlWriter xmlWriter = XmlWriter.Create(@"decryptedXmls\" + countryName + ".xml", settings))
                        xmlreader.WriteTo(xmlWriter);
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    return false;
                }

                if(!GetVehiclesFromXMLDoc(xmlreader, countryid, getTextProvider))
                    return false;
            }

            AutoReapir();

            TextBoxConsole.WriteLine(endl.NewLine + "坦克解析完成，共获得" + data.Count + "辆车辆的数据");
            return true;
        }

        public static void GetVehiclesXMLFile(string xmlPath, int countryid, string TEXTDOMAIN, GetTextProvider getTextProvider)
        {
            XmlDocument xmlreader = new XmlDocument();
            try
            {
                xmlreader.Load(xmlPath);
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message+ endl.NewLine +"Xml载入失败！");
                return;
            }
            getTextProvider.TEXTDOMAIN = currentTEXTDOMAIN = TEXTDOMAIN;

            if(!GetVehiclesFromXMLDoc(xmlreader, countryid, getTextProvider))
                return;
            TextBoxConsole.WriteLine(endl.NewLine + "坦克数据解析完成，共获得" + data.Count + "辆车辆的数据");
        }

        public static bool GetVehiclesFromXMLDoc(XmlDocument xmlreader, int countryid, GetTextProvider getTextProvider)
        {
            string tagsStr;
            bool IGRflag;
            try
            {
                foreach (XmlNode currentVehicle in xmlreader.FirstChild.ChildNodes)
                {
                    Vehicles currentResult = new Vehicles();
                    currentResult.id = Int32.Parse(currentVehicle.SelectSingleNode("id").FirstChild.Value);
                    currentResult.countryid = countryid;
                    currentResult.title = currentVehicle.SelectSingleNode("userString").FirstChild.Value.Split(':')[1];
                    if (IGRflag = currentResult.title.Contains("_IGR"))
                        getTextProvider.TEXTDOMAIN = "igr_vehicles";
                    currentResult.loacl_name = getTextProvider.GetText(currentResult.title);
                    currentResult.descr = getTextProvider.GetText(currentVehicle.SelectSingleNode("description").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText("_descr"))
                        errorCounter++;
                    if (currentVehicle.SelectSingleNode("shortUserString") == null)
                        currentResult.short_name = currentResult.loacl_name;
                    else
                    {
                        currentResult.short_name = getTextProvider.GetText(currentVehicle.SelectSingleNode("shortUserString").FirstChild.Value.Split(':')[1]);
                        if (!getTextProvider.CheckText("_short"))
                            errorCounter++;
                    }
                    if (IGRflag)
                        getTextProvider.TEXTDOMAIN = currentTEXTDOMAIN;
                    currentResult.icon_orig = currentVehicle.Name;
                    currentResult.tier = Int32.Parse(currentVehicle.SelectSingleNode("level").FirstChild.Value);
                    tagsStr = currentVehicle.SelectSingleNode("tags").FirstChild.Value;
                    if (tagsStr.Contains("lightTank"))
                    {
                        currentResult.type = 1;
                        TextBoxConsole.WriteLine(string.Format("解析到 {0,-3}级 LT  {1}", currentResult.tier, currentResult.loacl_name));
                    }
                    else if (tagsStr.Contains("mediumTank"))
                    {
                        currentResult.type = 2;
                        TextBoxConsole.WriteLine(string.Format("解析到 {0,-3}级 MT  {1}", currentResult.tier, currentResult.loacl_name));
                    }
                    else if (tagsStr.Contains("heavyTank"))
                    {
                        currentResult.type = 3;
                        TextBoxConsole.WriteLine(string.Format("解析到 {0,-3}级 HT  {1}", currentResult.tier, currentResult.loacl_name));
                    }
                    else if (tagsStr.Contains("AT-SPG"))
                    {
                        currentResult.type = 4;
                        TextBoxConsole.WriteLine(string.Format("解析到 {0,-3}级 TD  {1}", currentResult.tier, currentResult.loacl_name));
                    }
                    else if (tagsStr.Contains("SPG"))
                    {
                        currentResult.type = 5;
                        TextBoxConsole.WriteLine(string.Format("解析到 {0,-3}级 SPG {1}", currentResult.tier, currentResult.loacl_name));
                    }
                    else if (tagsStr.Contains("observer"))
                    {
                        currentResult.type = 1;
                        TextBoxConsole.WriteLine("解析到 观察者");
                    }
                    data.Add(currentResult);
                }
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "解析过程已终止，请检查xml格式是否正确");
                return false;
            }
            return true;
        }

        public static void AutoReapir()
        {
            foreach (Vehicles currentResult in data)
            {
                if (currentResult.title == "Env_Artillery" && currentResult.loacl_name == currentResult.title)
                {
                    currentResult.loacl_name = currentResult.short_name = "国服火炮";
                    currentResult.descr = "国服火炮世界第一";
                    TextBoxConsole.WriteLine(endl.NewLine + "修复Env_Artillery");
                    errorCounter--;
                }
                else if (currentResult.title == "Sexton_I" && currentResult.loacl_name == currentResult.title)
                {
                    foreach (Vehicles searchSexton in data)
                        if (searchSexton.icon_orig == "GB78_Sexton_I")
                        {
                            currentResult.loacl_name = searchSexton.loacl_name;
                            currentResult.short_name = searchSexton.short_name;
                            currentResult.descr = searchSexton.descr;
                            TextBoxConsole.WriteLine(endl.NewLine + "修复Sexton_I");
                            errorCounter--;
                        }
                }
            }
            if (errorCounter != 0)
                TextBoxConsole.WriteLine(endl.NewLine + "经过自动修复，仍有" + errorCounter + "个错误，请手动检查导出文件");
        }

        public static void WriteVehicles(int method, string outputPath, bool columnHead)
        {
            if (method == 0)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "开始写入Excel文件");
                Excel.Application excelapp;
                Excel.Workbook excelbook;
                Excel.Worksheet excelsheet;
                int writeCounter = 0;

                try
                {
                    excelapp = new Excel.Application();
                    excelapp.Visible = true;
                    excelbook = excelapp.Workbooks.Add();
                    excelsheet = excelbook.Sheets[1];
                    if (columnHead)
                    {
                        writeCounter = 1;
                        excelsheet.Cells[writeCounter, 1] = "车辆ID";
                        excelsheet.Cells[writeCounter, 2] = "国家ID";
                        excelsheet.Cells[writeCounter, 3] = "车辆在录像中的名称";
                        excelsheet.Cells[writeCounter, 4] = "车辆的显示名称";
                        excelsheet.Cells[writeCounter, 5] = "车辆的短名称";
                        excelsheet.Cells[writeCounter, 6] = "车辆的介绍";
                        excelsheet.Cells[writeCounter, 7] = "车辆图标与文件数据名称";
                        excelsheet.Cells[writeCounter, 8] = "车辆等级";
                        excelsheet.Cells[writeCounter, 9] = "车辆类别";
                    }
                    foreach (Vehicles currentResult in data)
                    {
                        writeCounter++;
                        excelsheet.Cells[writeCounter, 1] = currentResult.id;
                        excelsheet.Cells[writeCounter, 2] = currentResult.countryid;
                        excelsheet.Cells[writeCounter, 3] = currentResult.title;
                        excelsheet.Cells[writeCounter, 4] = currentResult.loacl_name;
                        excelsheet.Cells[writeCounter, 5] = currentResult.short_name;
                        excelsheet.Cells[writeCounter, 6] = currentResult.descr;
                        excelsheet.Cells[writeCounter, 7] = currentResult.icon_orig;
                        excelsheet.Cells[writeCounter, 8] = currentResult.tier;
                        excelsheet.Cells[writeCounter, 9] = currentResult.type;
                    }
                    excelapp.DisplayAlerts = false;
                    excelbook.SaveAs(outputPath + @"\Vehicles.xlsx");
                    excelbook.Close();
                    excelapp.Quit();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\Vehicles.xlsx");
                    TextBoxConsole.WriteLine("请注意在任务管理器清理后台的Excel.exe");
                    return;
                }

                TextBoxConsole.WriteLine(endl.NewLine + "车辆数据已写入到" + outputPath + @"\Vehicles.xlsx");
            }
            else if (method == 1)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "开始写入Json文件");
                JArray JsonData = new JArray();
                int writeCounter = 0;
                foreach (Vehicles currentResult in data)
                {
                    JsonData.Add(new JObject());
                    JsonData[writeCounter]["id"] = currentResult.id;
                    JsonData[writeCounter]["countryid"] = currentResult.countryid;
                    JsonData[writeCounter]["title"] = currentResult.title;
                    JsonData[writeCounter]["loacl_name"] = currentResult.loacl_name;
                    JsonData[writeCounter]["short_name"] = currentResult.short_name;
                    JsonData[writeCounter]["descr"] = currentResult.descr;
                    JsonData[writeCounter]["icon_orig"] = currentResult.icon_orig;
                    JsonData[writeCounter]["tier"] = currentResult.tier;
                    JsonData[writeCounter]["type"] = currentResult.type;
                    writeCounter++;
                }
                try
                {
                    StreamWriter sw = new StreamWriter(outputPath + @"\Vehicles.json", false, Encoding.UTF8);
                    sw.Write(JsonConvert.SerializeObject(data));
                    sw.Close();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\Vehicles.json");
                }
                TextBoxConsole.WriteLine(endl.NewLine + "车辆数据已写入到" + outputPath + @"\Vehicles.json");
            }
        }

        public static void Reset()
        {
            errorCounter = 0;
            currentTEXTDOMAIN = null;
            data.Clear();
        }
    }
}
