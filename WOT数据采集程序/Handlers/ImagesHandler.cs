using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using endl = System.Environment;

namespace WOTDataCollector
{
    class ImagesHandler
    {
        public static bool Ready(string wotPath)
        {
            try
            {
                if (Directory.Exists(@"encryptedXmls\gui"))
                {
                    TextBoxConsole.WriteLine(endl.NewLine + "删除encryptedXmls\\gui,该过程比较缓慢，程序可能无响应，请耐心等待……");
                    Directory.Delete(@"encryptedXmls\gui", true);
                }
                TextBoxConsole.WriteLine(endl.NewLine + "正在解压gui.pkg,该过程比较缓慢，程序可能无响应，请耐心等待……");
                ZipFile.OpenRead(wotPath + @"\res\packages\gui.pkg").ExtractToDirectory(@"encryptedXmls");
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }
            return true;
        }

        private static bool CheckDir(string outputPath, string tgtDir)
        {
            try
            {
                if (!Directory.Exists(outputPath + @"\images"))
                    Directory.CreateDirectory(outputPath + @"\images");
                if (Directory.Exists(outputPath + @"\images\" + tgtDir))
                {
                    TextBoxConsole.WriteLine(endl.NewLine + "删除" + outputPath + @"\images\" + tgtDir + "该过程比较缓慢，程序可能无响应，请耐心等待……");
                    Directory.Delete(outputPath + @"\images\" + tgtDir, true);
                }
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }
            return true;
        }

        public static void GetAll(string outputPath)
        {
            try
            {
                if (Directory.Exists(outputPath + @"\AllImages"))
                    Directory.Delete(outputPath + @"\AllImages", true);
                Directory.Move(@"encryptedXmls\gui\maps", outputPath + @"\AllImages");
                TextBoxConsole.WriteLine(endl.NewLine + "已经导出所有图片");
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                if (e.Message.Contains("移动操作在卷之间无效。"))
                    TextBoxConsole.WriteLine("要使用图片导出功能，请将导出目录选在程序所在的分区！如当前程序在E盘上，导出目录也必须在E盘。");
            }
        }

        public static void GetIcons(string outputPath, string part)
        {
            try
            {
                if (!CheckDir(outputPath, part))
                    return;
                Directory.Move(@"encryptedXmls\gui\maps\icons\" + part, outputPath + @"\images\" + part);
                TextBoxConsole.WriteLine(endl.NewLine + "已经导出" + part);

            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                if (e.Message.Contains("移动操作在卷之间无效。"))
                    TextBoxConsole.WriteLine("要使用图片导出功能，请将导出目录选在程序所在的分区！如当前程序在E盘上，导出目录也必须在E盘。");
            }
        }
    }
}
