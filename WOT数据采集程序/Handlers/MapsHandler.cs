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
    class MapsHandler
    {
        private class Maps
        {
            public int mapid;
            public string mapidname;
            public string mapname;
            public string mapdescr;
        }

        private static List<Maps> data = new List<Maps>();

        static int errorCounter = 0;

        public static bool GetMaps(GetTextProvider getTextProvider)

        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出地图数据……" + endl.NewLine);

            XmlDocument xmlreader = new XmlDocument();
            XmlDecompiler xmlDecompiler = XmlDecompiler.Instance;
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            getTextProvider.TEXTDOMAIN = "arenas";

            try
            {
                xmlreader.LoadXml(xmlDecompiler.GetFileXml(@"encryptedXmls\scripts\arena_defs\_list_.xml"));

                using (XmlWriter xmlWriter = XmlWriter.Create(@"decryptedXmls\arenas.xml", settings))
                    xmlreader.WriteTo(xmlWriter);
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }
            if (!GetMapsFromXMLDoc(xmlreader, getTextProvider))
                return false;

            TextBoxConsole.WriteLine(endl.NewLine + "地图解析完成，共获得" + data.Count + "张地图的数据");
            return true;
        }

        public static bool GetMapsXMLFile(string xmlPath, string TEXTDOMAIN, GetTextProvider getTextProvider)
        {
            Reset();
            TextBoxConsole.WriteLine(endl.NewLine + "正在导出地图数据……" + endl.NewLine);

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

            if (!GetMapsFromXMLDoc(xmlreader, getTextProvider))
                return false;
            TextBoxConsole.WriteLine(endl.NewLine + "地图数据解析完成，共获得" + data.Count + "张地图的数据");
            return true;
        }

        public static bool GetMapsFromXMLDoc(XmlDocument xmlreader, GetTextProvider getTextProvider)
        {
            try
            {
                foreach (XmlNode currentMap in xmlreader.FirstChild.ChildNodes)
                {
                    Maps currentResult = new Maps();
                    currentResult.mapid = Int32.Parse(currentMap.SelectSingleNode("id").FirstChild.Value);
                    currentResult.mapidname = currentMap.SelectSingleNode("name").FirstChild.Value;
                    currentResult.mapname = getTextProvider.GetText(currentResult.mapidname + "/name");
                    if (!getTextProvider.CheckText("/name"))
                        errorCounter++;
                    currentResult.mapdescr = getTextProvider.GetText(currentResult.mapidname + "/description");
                    if (!getTextProvider.CheckText("/description"))
                        errorCounter++;
                    TextBoxConsole.WriteLine("解析到 " + currentResult.mapname);
                    data.Add(currentResult);
                }
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message + endl.NewLine + "解析过程已终止，请检查xml格式是否正确");
                return false;
            }
            if (errorCounter != 0)
                TextBoxConsole.WriteLine(endl.NewLine + "共发现" + errorCounter + "个错误，请手动检查导出文件");
            return true;
        }

        public static void WriteMaps(int method, string outputPath, bool columnHead)
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
                        excelsheet.Cells[writeCounter, 1] = "地图ID";
                        excelsheet.Cells[writeCounter, 2] = "地图在录像中的名称";
                        excelsheet.Cells[writeCounter, 3] = "地图的显示名称";
                        excelsheet.Cells[writeCounter, 4] = "地图的介绍";
                    }
                    foreach (Maps currentResult in data)
                    {
                        writeCounter++;
                        excelsheet.Cells[writeCounter, 1] = currentResult.mapid;
                        excelsheet.Cells[writeCounter, 2] = currentResult.mapidname;
                        excelsheet.Cells[writeCounter, 3] = currentResult.mapname;
                        excelsheet.Cells[writeCounter, 4] = currentResult.mapdescr;
                    }
                    excelapp.DisplayAlerts = false;
                    excelbook.SaveAs(outputPath + @"\Maps.xlsx");
                    excelbook.Close();
                    excelapp.Quit();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\Maps.xlsx");
                    TextBoxConsole.WriteLine("请注意在任务管理器清理后台的Excel.exe");
                    return;
                }

                TextBoxConsole.WriteLine(endl.NewLine + "地图数据已写入到" + outputPath + @"\Maps.xlsx");
            }
            else if (method == 1)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "开始写入Json文件");
                JArray JsonData = new JArray();
                int writeCounter = 0;
                foreach (Maps currentResult in data)
                {
                    JsonData.Add(new JObject());
                    JsonData[writeCounter]["mapid"] = currentResult.mapid; ;
                    JsonData[writeCounter]["mapidname"] = currentResult.mapidname;
                    JsonData[writeCounter]["mapname"] = currentResult.mapname;
                    JsonData[writeCounter]["mapdescr"] = currentResult.mapdescr;
                    writeCounter++;
                }
                try
                {
                    StreamWriter sw = new StreamWriter(outputPath + @"\Maps.json", false, Encoding.UTF8);
                    sw.Write(JsonConvert.SerializeObject(data));
                    sw.Close();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("文件写入失败！");
                    TextBoxConsole.WriteLine("当前导出文件：" + outputPath + @"\Maps.json");
                }
                TextBoxConsole.WriteLine(endl.NewLine + "地图数据已写入到" + outputPath + @"\Maps.json");
            }
        }

        public static void Reset()
        {
            errorCounter = 0;
            data.Clear();
        }
    }
}
