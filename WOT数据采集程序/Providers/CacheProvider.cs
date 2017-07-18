using System;
using System.IO;
using System.Globalization;
using System.IO.Compression;
using System.Diagnostics;
using endl = System.Environment;

namespace WOT数据采集程序
{
    class CacheProvider
    {
        public static bool GetMOandXml(string wotPath)
        {
            TextBoxConsole.WriteLine(endl.NewLine + "创建文本数据缓存");
            try
            {
                string destPath = @"MO\" + CultureInfo.CurrentUICulture.Name.Replace('-', '_') + @"\LC_MESSAGES";
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(@"MO");
                    Directory.CreateDirectory(@"MO\" + CultureInfo.CurrentUICulture.Name.Replace('-', '_'));
                    Directory.CreateDirectory(destPath);
                    TextBoxConsole.WriteLine(endl.NewLine + "创建MO缓存目录：" + Path.GetFullPath(destPath));
                }

                string[] files = Directory.GetFiles(wotPath + @"\res\text\LC_MESSAGES");
                TextBoxConsole.WriteLine(endl.NewLine + "LC_MESSAGES所在路径：" + wotPath + @"\res\text\LC_MESSAGES" + endl.NewLine);
                foreach (string s in files)
                {
                    string fileName = Path.GetFileName(s);
                    File.Copy(s, destPath + @"\" + fileName, true);
                    TextBoxConsole.WriteLine("复制" + Path.GetFileName(s));
                }

                if (!Directory.Exists(@"encryptedXmls"))
                {
                    Directory.CreateDirectory(@"encryptedXmls");
                    TextBoxConsole.WriteLine(endl.NewLine + "创建加密xml缓存目录：" + Path.GetFullPath("encryptedXmls"));
                }
                if (!Directory.Exists(@"decryptedXmls"))
                {
                    Directory.CreateDirectory(@"decryptedXmls");
                    TextBoxConsole.WriteLine(endl.NewLine + "创建解密xml缓存目录：" + Path.GetFullPath("decryptedXmls"));
                }

                if (Directory.Exists(@"encryptedXmls\scripts"))
                {
                    TextBoxConsole.WriteLine(endl.NewLine + "删除encryptedXmls\\scripts,该过程比较缓慢，程序可能无响应，请耐心等待……");
                    Directory.Delete(@"encryptedXmls\scripts", true);
                }
                TextBoxConsole.WriteLine(endl.NewLine + "正在解压scripts.pkg,该过程比较缓慢，程序可能无响应，请耐心等待……");
                ZipFile.OpenRead(wotPath + @"\res\packages\scripts.pkg").ExtractToDirectory(@"encryptedXmls");
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }
            return true;
        }

        public static bool GetMO(string MOPath)
        {
            string destPath = @"MO\" + CultureInfo.CurrentUICulture.Name.Replace('-', '_') + @"\LC_MESSAGES";
            try
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(@"MO");
                    Directory.CreateDirectory(@"MO\" + CultureInfo.CurrentUICulture.Name.Replace('-', '_'));
                    Directory.CreateDirectory(destPath);
                    TextBoxConsole.WriteLine(endl.NewLine + "创建MO缓存目录：" + Path.GetFullPath(destPath) + endl.NewLine);
                }

                File.Copy(MOPath, destPath + @"\" + Path.GetFileName(MOPath), true);
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                return false;
            }
            TextBoxConsole.WriteLine("复制" + Path.GetFileName(MOPath));
            return true;
        }

        private static Process p = new Process();

        public static bool CheckPython()
        {
            p.StartInfo.FileName = "python";
            p.StartInfo.Arguments = " -V";
            p.StartInfo.UseShellExecute = false;//不使用系统外壳程序启动 
            p.StartInfo.RedirectStandardInput = true;//可以重定向输入  
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;//不创建窗口

            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "似乎你并没有安装Python2.7 成就导出将不可用");
                TextBoxConsole.WriteLine("安装时Python注意选中Add python.exe to Path");
                TextBoxConsole.WriteLine("如果你确认自己已经安装了Python2.7 请检查是否正确地添加了环境变量");
                TextBoxConsole.WriteLine(e.Message);
                return false;
            }
            p.WaitForExit(5000);
            string returnStr = p.StandardError.ReadToEnd();
            TextBoxConsole.Write(endl.NewLine + endl.NewLine + "检测到" + returnStr);

            string[] versionStrs = returnStr.Split(' ')[1].Split('.');
            if (Int32.Parse(versionStrs[0]) != 2 || Int32.Parse(versionStrs[1]) != 7)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "你安装的不是Python2.7 成就导出将不可用");
                return false;
            }

            TextBoxConsole.WriteLine(endl.NewLine + "正在检测是否安装了uncompyle2，这可能需要一点时间……");
            p.StartInfo.Arguments = "-c help('modules')";
            p.Start();
            p.WaitForExit(3000);
            returnStr = p.StandardOutput.ReadToEnd();

            if (!returnStr.Contains("uncompyle2"))
            {
                TextBoxConsole.WriteLine(endl.NewLine + "你并没有安装uncompyle2或者Python未能在3秒内返回模块列表，现在开始安装，这可能需要一点时间……重复安装模块不会导致错误，请放心");
                p.StartInfo.Arguments = Path.GetFullPath(@"uncompyle2\setup.py") + " install";
                p.StartInfo.WorkingDirectory = Path.GetFullPath(@"uncompyle2");
                try
                {
                    p.Start();
                }
                catch (Exception e)
                {
                    TextBoxConsole.WriteLine(endl.NewLine + e.Message);
                    TextBoxConsole.WriteLine("似乎你的Python有毒 成就导出将不可用");
                    return false;
                }
                p.WaitForExit(10000);
                TextBoxConsole.Write(endl.NewLine + p.StandardOutput.ReadToEnd());

                returnStr = p.StandardError.ReadToEnd();
                if (returnStr != "")
                {
                    TextBoxConsole.WriteLine("似乎在安装uncompyle2的过程中出现了错误，请根据下列信息自行判断是否正确安装，如果没有正确安装，请不要使用成就导出功能");
                    TextBoxConsole.Write(returnStr);
                }

                TextBoxConsole.WriteLine(endl.NewLine + "uncompyle2安装完成");
            }

            TextBoxConsole.WriteLine(endl.NewLine + "Python平台就绪！");
            return true;
        }

        public static bool DecompilePYC(string PYCPath = null)
        {
            string _PYCPath;
            try
            {
                if (PYCPath != null)
                    _PYCPath = Path.GetFullPath(PYCPath);
                else
                    _PYCPath = Path.GetFullPath(@"encryptedXmls\scripts\common\dossiers2\custom\records.pyc");
                p.StartInfo.Arguments = Path.GetFullPath(@"uncompyle2\scripts\uncompyle2") + " -o " + Path.GetFullPath(@"decryptedXmls\records.py") + " -d --py --verify " + _PYCPath;

                TextBoxConsole.WriteLine(endl.NewLine + "反编译" + Path.GetFileName(_PYCPath));

                p.Start();
            }
            catch (PathTooLongException e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "路径过长，请将程序放在根目录下运行");
                TextBoxConsole.WriteLine(e.Message);
                return false;
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine(endl.NewLine + "反编译失败，似乎Python炸了");
                TextBoxConsole.WriteLine(e.Message);
                return false;
            }
            p.WaitForExit();

            string returnStr = p.StandardError.ReadToEnd();
            if (returnStr != "")
            {
                TextBoxConsole.WriteLine(endl.NewLine + Path.GetFileName(_PYCPath) + "反编译失败，以下是错误信息");
                TextBoxConsole.Write(returnStr);
                return false;
            }

            TextBoxConsole.Write(p.StandardOutput.ReadToEnd());
            return true;
        }
    }
}
