using System;
using System.Collections.Generic;
using System.Text;
using endl = System.Environment;
using System.IO;
using System.Xml;
using wottoolslib;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WOTDataCollector
{
    class AchievementsHandler
    {
        private class Achievements
        {
            public int id;
            public bool withRibbon=false;
            public bool right=false;
            public string iconName;
            public string name;
            public string descr;
            public string condition;
            public string heroInfo;
        }

        private static List<Achievements> data = new List<Achievements>();

        static int errorCounter = 0;

        public static bool GetAchievements(GetTextProvider getTextProvider)
        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出成就数据……");

            XmlDocument xmlreader = new XmlDocument();
            XmlDecompiler xmlDecompiler = XmlDecompiler.Instance;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            getTextProvider.TEXTDOMAIN = "achievements";

            try
            {
                xmlreader.LoadXml(xmlDecompiler.GetFileXml(@"encryptedXmls\scripts\item_defs\achievements.xml"));

                using (XmlWriter xmlWriter = XmlWriter.Create(@"decryptedXmls\achievements.xml", settings))
                    xmlreader.WriteTo(xmlWriter);

            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }

            if (!CacheProvider.DecompilePYC())
                return false;

            try
            {
                using (StreamReader pyreader = new StreamReader(@"decryptedXmls\records.py"))
                    if (!GetAchievementsFromXMLDoc(xmlreader, pyreader, getTextProvider))
                        return false;

            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }

            TextBoxConsole.WriteLine(endl.NewLine + "成就解析完成，共获得" + data.Count + "个成就的数据");
            return true;
        }

        public static bool GetAchievementsXMLFile(string xmlPath,string pyPath, string TEXTDOMAIN, GetTextProvider getTextProvider)
        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出成就数据……");

            XmlDocument xmlreader = new XmlDocument();

            try
            {
                xmlreader.Load(xmlPath);
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "Xml载入失败！");
                return false;
            }
            getTextProvider.TEXTDOMAIN = TEXTDOMAIN;

            try
            {
                using (StreamReader pyreader = new StreamReader(pyPath))
                    if (!GetAchievementsFromXMLDoc(xmlreader, pyreader, getTextProvider))
                        return false;

            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }

            TextBoxConsole.WriteLine(endl.NewLine + "成就解析完成，共获得" + data.Count + "个成就的数据");
            return true;
        }

        public static bool GetAchievementsFromXMLDoc(XmlDocument xmlreader, StreamReader pyreader, GetTextProvider getTextProvider)
        {
            string pyLine=null;
            try
            {
                while (!(pyLine = pyreader.ReadLine()).Contains("RECORD_DB_IDS")) ;
            }
            catch(EndOfStreamException e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                TextBoxConsole.WriteLine("读取到文件尾，未能在py文件中找到RECORD_DB_IDS");
                return false;
            }
            catch(Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                TextBoxConsole.WriteLine("py文件读取失败！");
                return false;
            }

            char[] trimChars = { ',', '}' };
            char[] trimClassChars = { ' ', '，' };
            string[] splitLines;
            string name;
            bool findFlag;
            Achievements currentResult=null;

            for (;;)
            {
                findFlag = false;
                splitLines = pyLine.Split('\'');
                name = splitLines[1] + ":" + splitLines[3];
                try
                {
                    foreach (XmlNode currentAchievement in xmlreader.FirstChild.SelectSingleNode("achievements").ChildNodes)
                    {
                        if (currentAchievement.SelectSingleNode("name").FirstChild.Value != name)
                            continue;
                        if(float.Parse( currentAchievement.SelectSingleNode("value").ChildNodes[2].FirstChild.Value)<=0)
                            break;
                        findFlag = true;
                        currentResult = new Achievements();
                        foreach(XmlNode ribbonNode in xmlreader.FirstChild.SelectSingleNode("battleAchievesWithRibbon").FirstChild.ChildNodes)
                            if (ribbonNode.FirstChild.Value.Replace("	", "") == splitLines[3])
                            {
                                currentResult.withRibbon = true;
                                xmlreader.FirstChild.SelectSingleNode("battleAchievesWithRibbon").FirstChild.RemoveChild(ribbonNode);
                                break;
                            }
                        foreach (XmlNode rightNode in xmlreader.FirstChild.SelectSingleNode("battleResultsRight").FirstChild.ChildNodes)
                            if (rightNode.FirstChild.Value.Replace("	", "") == splitLines[3])
                            {
                                currentResult.right = true;
                                xmlreader.FirstChild.SelectSingleNode("battleResultsRight").FirstChild.RemoveChild(rightNode);
                                break;
                            }
                        foreach (XmlNode rightNode in xmlreader.FirstChild.SelectSingleNode("fortBattleResultsRight").FirstChild.ChildNodes)
                            if (rightNode.FirstChild.Value.Replace("	", "") == splitLines[3])
                            {
                                currentResult.right = true;
                                xmlreader.FirstChild.SelectSingleNode("fortBattleResultsRight").FirstChild.RemoveChild(rightNode);
                                break;
                            }
                        currentResult.iconName = splitLines[3];
                        currentResult.name = getTextProvider.GetText(currentResult.iconName);
                        if (!getTextProvider.CheckText(currentResult.iconName))
                            errorCounter++;
                        //if (currentAchievement.SelectSingleNode("value").ChildNodes[0].FirstChild.Value == "class")
                        //{
                        //    currentResult.name = currentResult.name.Replace("%(rank)s", "");
                        //    currentResult.name = currentResult.name.TrimEnd(trimClassChars);
                        //}
                        currentResult.descr = getTextProvider.GetText(currentResult.iconName + "_descr");
                        if (currentResult.descr.Contains("_descr"))
                            currentResult.descr="";
                        currentResult.condition = getTextProvider.GetText(currentResult.iconName + "_condition");
                        if (currentResult.condition.Contains("_condition"))
                            currentResult.condition = "";
                        if (currentResult.condition== "?empty?")
                            currentResult.condition = "";
                        currentResult.heroInfo = getTextProvider.GetText(currentResult.iconName + "_heroInfo");
                        if (currentResult.heroInfo.Contains("_heroInfo"))
                            currentResult.heroInfo = "";
                        xmlreader.FirstChild.SelectSingleNode("achievements").RemoveChild(currentAchievement);
                        break;
                    }
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "解析过程已终止，请检查xml格式是否正确");
                    return false;
                }
                if(findFlag)
                {
                    currentResult.id = int.Parse(pyLine.Split(':')[1].TrimEnd(trimChars));
                    TextBoxConsole.WriteLine("解析到 " + currentResult.name);
                    data.Add(currentResult);
                }
                if ( pyLine.EndsWith("}"))
                    break;
                try
                {
                    pyLine = pyreader.ReadLine();
                }
                catch (EndOfStreamException e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("意外读取到文件尾，RECORD_DB_IDS没有闭合，请检查导出文件！");
                    break;
                }
            }

            if (errorCounter != 0)
                TextBoxConsole.WriteLine(endl.NewLine + "共发现" + errorCounter + "个错误，请手动检查导出文件");
            return true;
        }

        public static void WriteAchievements(int method, string outputPath, bool columnHead)
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
                        excelsheet.Cells[writeCounter, 1] = "成就ID";
                        excelsheet.Cells[writeCounter, 2] = "有勋带";
                        excelsheet.Cells[writeCounter, 3] = "在右边显示";
                        excelsheet.Cells[writeCounter, 4] = "图标文件名称";
                        excelsheet.Cells[writeCounter, 5] = "成就名称";
                        excelsheet.Cells[writeCounter, 6] = "成就描述";
                        excelsheet.Cells[writeCounter, 7] = "成就获取限制";
                        excelsheet.Cells[writeCounter, 8] = "成就历史";
                    }
                    foreach (Achievements currentResult in data)
                    {
                        writeCounter++;
                        excelsheet.Cells[writeCounter, 1] = currentResult.id;
                        excelsheet.Cells[writeCounter, 2] = currentResult.withRibbon;
                        excelsheet.Cells[writeCounter, 3] = currentResult.right;
                        excelsheet.Cells[writeCounter, 4] = currentResult.iconName;
                        excelsheet.Cells[writeCounter, 5] = currentResult.name;
                        excelsheet.Cells[writeCounter, 6] = currentResult.descr;
                        excelsheet.Cells[writeCounter, 7] = currentResult.condition;
                        excelsheet.Cells[writeCounter, 8] = currentResult.heroInfo;
                    }
                    excelapp.DisplayAlerts = false;
                    excelbook.SaveAs(outputPath + @"\Achievements.xlsx");
                    excelbook.Close();
                    excelapp.Quit();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\Achievements.xlsx");
                    TextBoxConsole.WriteLine("请注意在任务管理器清理后台的Excel.exe");
                    return;
                }

                TextBoxConsole.WriteLine(endl.NewLine + "成就数据已写入到" + outputPath + @"\Achievements.xlsx");
            }
            else if (method == 1)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "开始写入Json文件");
                JArray JsonData = new JArray();
                int writeCounter = 0;
                foreach (Achievements currentResult in data)
                {
                    JsonData.Add(new JObject());
                    JsonData[writeCounter]["id"] = currentResult.id; ;
                    JsonData[writeCounter]["withRibbon"] = currentResult.withRibbon;
                    JsonData[writeCounter]["right"] = currentResult.right;
                    JsonData[writeCounter]["iconName"] = currentResult.iconName;
                    JsonData[writeCounter]["name"] = currentResult.name;
                    JsonData[writeCounter]["descr"] = currentResult.descr;
                    JsonData[writeCounter]["condition"] = currentResult.condition;
                    JsonData[writeCounter]["heroInfo"] = currentResult.heroInfo;
                    writeCounter++;
                }
                try
                {
                    StreamWriter sw = new StreamWriter(outputPath + @"\Achievements.json", false, Encoding.UTF8);
                    sw.Write(JsonConvert.SerializeObject(data));
                    sw.Close();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\Achievements.json");
                }
                TextBoxConsole.WriteLine(endl.NewLine + "成就数据已写入到" + outputPath + @"\Achievements.json");
            }
        }

        public static void Reset()
        {
            errorCounter = 0;
            data.Clear();
        }
    }
}
