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
    class PersonalMissionsHandler
    {
        private class Seasons
        {
            public int id;
            public string name;
        }

        private class Tiles
        {
            public int id;
            public int seasonId;
            public string tileName;
            public string seasonName;
        }

        private class Quests
        {
            public int id;
            public int seasonId;
            public int tileId;
            public int chainId;
            public int missionId;
            public string seasonName;
            public string tileName;
            public string missionName;
            public string mixNameShort;
            public string mixNameLong;
            public string mainTgt;
            public string addTgt;
            public int minLevel;
            public int maxLevel;
            public string advice;
            public string missionDescr;
            public string mainDescr;
            public string addDescr;
        }

        private static List<Seasons> seasonsData = new List<Seasons>();
        private static List<Tiles> tilesData = new List<Tiles>();
        private static List<Quests> data = new List<Quests>();

        static int errorCounter = 0;

        public static bool GetQuests(GetTextProvider getTextProvider)

        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出个人任务数据……");

            XmlDocument xmlreader = new XmlDocument();
            XmlDecompiler xmlDecompiler = XmlDecompiler.Instance;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            getTextProvider.TEXTDOMAIN = "personal_missions_details";

            try
            {
                xmlreader.LoadXml(xmlDecompiler.GetFileXml(@"encryptedXmls\scripts\item_defs\potapov_quests\seasons.xml"));

                using (XmlWriter xmlWriter = XmlWriter.Create(@"decryptedXmls\questsSeasons.xml", settings))
                    xmlreader.WriteTo(xmlWriter);

                if (!GetSeasonsFromXMLDoc(xmlreader, getTextProvider))
                    return false;

                xmlreader.LoadXml(xmlDecompiler.GetFileXml(@"encryptedXmls\scripts\item_defs\potapov_quests\tiles.xml"));

                using (XmlWriter xmlWriter = XmlWriter.Create(@"decryptedXmls\questsTiles.xml", settings))
                    xmlreader.WriteTo(xmlWriter);

                if (!GetTilesFromXMLDoc(xmlreader, getTextProvider))
                    return false;

                xmlreader.LoadXml(xmlDecompiler.GetFileXml(@"encryptedXmls\scripts\item_defs\potapov_quests\list.xml"));

                using (XmlWriter xmlWriter = XmlWriter.Create(@"decryptedXmls\quests.xml", settings))
                    xmlreader.WriteTo(xmlWriter);

                if (!GetQuestsFromXMLDoc(xmlreader, getTextProvider))
                    return false;
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }

            TextBoxConsole.WriteLine(endl.NewLine + "个人任务解析完成，共获得" + data.Count + "个任务的数据");
            return true;
        }

        public static bool GetQuestsXMLFile(string seasonsPath, string tilesPath, string questsPath, string TEXTDOMAIN, GetTextProvider getTextProvider)
        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出个人任务数据……");

            XmlDocument xmlreader = new XmlDocument();

            getTextProvider.TEXTDOMAIN = TEXTDOMAIN;

            try
            {
                xmlreader.Load(seasonsPath);

                if (!GetSeasonsFromXMLDoc(xmlreader, getTextProvider))
                    return false;
                xmlreader.Load(tilesPath);


                if (!GetTilesFromXMLDoc(xmlreader, getTextProvider))
                    return false;

                xmlreader.Load(questsPath);

                if (!GetQuestsFromXMLDoc(xmlreader, getTextProvider))
                    return false;
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "Xml载入失败！");
                return false;
            }

            TextBoxConsole.WriteLine(endl.NewLine + "个人任务解析完成，共获得" + data.Count + "个任务的数据");
            return true;
        }

        public static bool GetSeasonsFromXMLDoc(XmlDocument xmlreader, GetTextProvider getTextProvider)
        {
            TextBoxConsole.WriteLine(endl.NewLine + "开始解析个人任务Seasons" + endl.NewLine);
            try
            {
                foreach (XmlNode currentSeason in xmlreader.FirstChild.ChildNodes)
                {
                    Seasons currentResult = new Seasons();
                    currentResult.id = Int32.Parse(currentSeason.SelectSingleNode("id").FirstChild.Value);
                    currentResult.name = getTextProvider.GetText(currentSeason.SelectSingleNode("userString").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentSeason.SelectSingleNode("userString").FirstChild.Value.Split(':')[1]))
                        errorCounter++;
                    TextBoxConsole.WriteLine("解析到 " + currentResult.name);
                    seasonsData.Add(currentResult);
                }
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "解析过程已终止，请检查xml格式是否正确");
                return false;
            }
            TextBoxConsole.WriteLine(endl.NewLine + "共获得" + seasonsData.Count + "个Seasons");
            return true;
        }

        public static bool GetTilesFromXMLDoc(XmlDocument xmlreader, GetTextProvider getTextProvider)
        {
            TextBoxConsole.WriteLine(endl.NewLine + "开始解析个人任务Tiles" + endl.NewLine);
            try
            {
                foreach (XmlNode currentTile in xmlreader.FirstChild.ChildNodes)
                {
                    if (currentTile.SelectSingleNode("id") == null)
                        break;
                    Tiles currentResult = new Tiles();
                    currentResult.id = Int32.Parse(currentTile.SelectSingleNode("id").FirstChild.Value);
                    currentResult.seasonId = Int32.Parse(currentTile.SelectSingleNode("seasonID").FirstChild.Value);
                    currentResult.tileName = getTextProvider.GetText(currentTile.SelectSingleNode("userString").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentTile.SelectSingleNode("userString").FirstChild.Value.Split(':')[1]))
                        errorCounter++;
                    foreach (Seasons currentSeason in seasonsData)
                        if (currentSeason.id == currentResult.seasonId)
                            currentResult.seasonName = currentSeason.name;
                    TextBoxConsole.WriteLine("解析到 " + currentResult.tileName);
                    tilesData.Add(currentResult);
                }
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "解析过程已终止，请检查xml格式是否正确");
                return false;
            }
            TextBoxConsole.WriteLine(endl.NewLine + "共获得" + tilesData.Count + "个Tiles");
            return true;
        }

        public static bool GetQuestsFromXMLDoc(XmlDocument xmlreader, GetTextProvider getTextProvider)
        {
            TextBoxConsole.WriteLine(endl.NewLine + "开始解析个人任务Quests" + endl.NewLine);
            try
            {
                foreach (XmlNode currentQuest in xmlreader.FirstChild.ChildNodes)
                {
                    Quests currentResult = new Quests();
                    currentResult.id = Int32.Parse(currentQuest.SelectSingleNode("id").FirstChild.Value);
                    currentResult.tileId = Int32.Parse(currentQuest.Name.Split('_')[1]);
                    foreach (Tiles currentTile in tilesData)
                        if (currentTile.id == currentResult.tileId)
                        {
                            currentResult.seasonId = currentTile.seasonId;
                            currentResult.tileName = currentTile.tileName;
                            currentResult.seasonName = currentTile.seasonName;
                        }
                    currentResult.chainId = Int32.Parse(currentQuest.Name.Split('_')[2]);
                    currentResult.missionId = Int32.Parse(currentQuest.Name.Split('_')[3]);
                    currentResult.missionName = getTextProvider.GetText(currentQuest.SelectSingleNode("userString").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentQuest.SelectSingleNode("userString").FirstChild.Value.Split(':')[1]))
                        errorCounter++;
                    currentResult.mixNameShort = currentResult.tileName + ":" + currentResult.missionName.Split(':')[0];
                    currentResult.mixNameLong = currentResult.seasonName + ":" + currentResult.tileName + ":" + currentResult.missionName;
                    currentResult.mainTgt = currentQuest.Name + "_main";
                    currentResult.addTgt = currentQuest.Name + "_add";
                    currentResult.minLevel = Int32.Parse(currentQuest.SelectSingleNode("minLevel").FirstChild.Value);
                    currentResult.maxLevel = Int32.Parse(currentQuest.SelectSingleNode("maxLevel").FirstChild.Value);
                    currentResult.advice = getTextProvider.GetText(currentQuest.SelectSingleNode("advice").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentQuest.SelectSingleNode("advice").FirstChild.Value.Split(':')[1]))
                        errorCounter++;
                    currentResult.missionDescr = getTextProvider.GetText(currentQuest.SelectSingleNode("description").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentQuest.SelectSingleNode("description").FirstChild.Value.Split(':')[1]))
                        errorCounter++;
                    currentResult.mainDescr = getTextProvider.GetText(currentQuest.SelectSingleNode("condition_main").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentQuest.SelectSingleNode("condition_main").FirstChild.Value.Split(':')[1]))
                        errorCounter++;
                    currentResult.addDescr = getTextProvider.GetText(currentQuest.SelectSingleNode("condition_add").FirstChild.Value.Split(':')[1]);
                    if (!getTextProvider.CheckText(currentQuest.SelectSingleNode("condition_add").FirstChild.Value.Split(':')[1]))
                        errorCounter++;

                    TextBoxConsole.WriteLine("解析到 " + currentResult.mixNameLong);
                    data.Add(currentResult);
                }
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "解析过程已终止，请检查xml格式是否正确");
                return false;
            }
            TextBoxConsole.WriteLine(endl.NewLine + "共获得" + data.Count + "个Quests");
            return true;
        }

        public static void WriteQuests(int method, string outputPath, bool columnHead)
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
                        excelsheet.Cells[writeCounter, 1] = "唯一任务ID";
                        excelsheet.Cells[writeCounter, 2] = "季度ID";
                        excelsheet.Cells[writeCounter, 3] = "砖块ID";
                        excelsheet.Cells[writeCounter, 4] = "系列ID";
                        excelsheet.Cells[writeCounter, 5] = "任务ID";
                        excelsheet.Cells[writeCounter, 6] = "季度名称";
                        excelsheet.Cells[writeCounter, 7] = "砖块名称";
                        excelsheet.Cells[writeCounter, 8] = "任务名称";
                        excelsheet.Cells[writeCounter, 9] = "混合短名称";
                        excelsheet.Cells[writeCounter, 10] = "混合长名称";
                        excelsheet.Cells[writeCounter, 11] = "主要条件标识符";
                        excelsheet.Cells[writeCounter, 12] = "第二条件标识符";
                        excelsheet.Cells[writeCounter, 13] = "最小等级";
                        excelsheet.Cells[writeCounter, 14] = "最大等级";
                        excelsheet.Cells[writeCounter, 15] = "任务提示";
                        excelsheet.Cells[writeCounter, 16] = "任务描述";
                        excelsheet.Cells[writeCounter, 17] = "主要条件描述";
                        excelsheet.Cells[writeCounter, 18] = "第二条件描述";
                    }
                    foreach (Quests currentResult in data)
                    {
                        writeCounter++;
                        excelsheet.Cells[writeCounter, 1] = currentResult.id;
                        excelsheet.Cells[writeCounter, 2] = currentResult.seasonId;
                        excelsheet.Cells[writeCounter, 3] = currentResult.tileId;
                        excelsheet.Cells[writeCounter, 4] = currentResult.chainId;
                        excelsheet.Cells[writeCounter, 5] = currentResult.missionId;
                        excelsheet.Cells[writeCounter, 6] = currentResult.seasonName;
                        excelsheet.Cells[writeCounter, 7] = currentResult.tileName;
                        excelsheet.Cells[writeCounter, 8] = currentResult.missionName;
                        excelsheet.Cells[writeCounter, 9] = currentResult.mixNameShort;
                        excelsheet.Cells[writeCounter, 10] = currentResult.mixNameLong;
                        excelsheet.Cells[writeCounter, 11] = currentResult.mainTgt;
                        excelsheet.Cells[writeCounter, 12] = currentResult.addTgt;
                        excelsheet.Cells[writeCounter, 13] = currentResult.minLevel;
                        excelsheet.Cells[writeCounter, 14] = currentResult.maxLevel;
                        excelsheet.Cells[writeCounter, 15] = currentResult.advice;
                        excelsheet.Cells[writeCounter, 16] = currentResult.missionDescr;
                        excelsheet.Cells[writeCounter, 17] = currentResult.mainDescr;
                        excelsheet.Cells[writeCounter, 18] = currentResult.addDescr;
                    }
                    excelapp.DisplayAlerts = false;
                    excelbook.SaveAs(outputPath + @"\PersonalMissions.xlsx");
                    excelbook.Close();
                    excelapp.Quit();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\PersonalMissions.xlsx");
                    TextBoxConsole.WriteLine("请注意在任务管理器清理后台的Excel.exe");
                    return;
                }

                TextBoxConsole.WriteLine(endl.NewLine + "个人任务数据已写入到" + outputPath + @"\PersonalMissions.xlsx");
            }
            else if (method == 1)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "开始写入Json文件");
                JArray JsonData = new JArray();
                int writeCounter = 0;
                foreach (Quests currentResult in data)
                {
                    JsonData.Add(new JObject());
                    JsonData[writeCounter]["id"] = currentResult.id; ;
                    JsonData[writeCounter]["seasonId"] = currentResult.seasonId;
                    JsonData[writeCounter]["tileId"] = currentResult.tileId;
                    JsonData[writeCounter]["chainId"] = currentResult.chainId;
                    JsonData[writeCounter]["missionId"] = currentResult.missionId;
                    JsonData[writeCounter]["seasonName"] = currentResult.seasonName;
                    JsonData[writeCounter]["tileName"] = currentResult.tileName;
                    JsonData[writeCounter]["missionName"] = currentResult.missionName;
                    JsonData[writeCounter]["mixNameShort"] = currentResult.mixNameShort;
                    JsonData[writeCounter]["mixNameLong"] = currentResult.mixNameLong;
                    JsonData[writeCounter]["mainTgt"] = currentResult.mainTgt;
                    JsonData[writeCounter]["addTgt"] = currentResult.addTgt;
                    JsonData[writeCounter]["minLevel"] = currentResult.minLevel;
                    JsonData[writeCounter]["maxLevel"] = currentResult.maxLevel;
                    JsonData[writeCounter]["advice"] = currentResult.advice;
                    JsonData[writeCounter]["missionDescr"] = currentResult.missionDescr;
                    JsonData[writeCounter]["mainDescr"] = currentResult.mainDescr;
                    JsonData[writeCounter]["addDescr"] = currentResult.addDescr;
                    writeCounter++;
                }
                try
                {
                    StreamWriter sw = new StreamWriter(outputPath + @"\PersonalMissions.json", false, Encoding.UTF8);
                    sw.Write(JsonConvert.SerializeObject(data));
                    sw.Close();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\PersonalMissions.json");
                }
                TextBoxConsole.WriteLine(endl.NewLine + "个人任务数据已写入到" + outputPath + @"\PersonalMissions.json");
            }
        }

        public static void Reset()
        {
            errorCounter = 0;
            seasonsData.Clear();
            tilesData.Clear();
            data.Clear();
        }
    }
}