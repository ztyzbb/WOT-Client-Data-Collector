using System;
using System.Diagnostics;

namespace WOT数据采集程序
{
    public class GetTextProvider
    {
        private Process p = new Process();
        private string _TEXTDOMAIN;
        public string TEXTDOMAIN
        {
            get { return _TEXTDOMAIN; }
            set { _TEXTDOMAIN = value + ' '; }
        }

        private string lastText;

        public GetTextProvider(string TEXTDOMAINDIR)
        {
            p.StartInfo.FileName = @"GetText\gettext.exe";
            p.StartInfo.UseShellExecute = false;//不使用系统外壳程序启动 
            p.StartInfo.RedirectStandardInput = true;//可以重定向输入  
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;//不创建窗口
            p.StartInfo.EnvironmentVariables.Add("TEXTDOMAINDIR", TEXTDOMAINDIR);
        }
        public string GetText(string src)
        {

            p.StartInfo.Arguments = _TEXTDOMAIN + src;
            try
            {
                p.Start();
                p.WaitForExit(1000);
            }
            catch (Exception e)
            {
                TextBoxConsole.WriteLine("======================警告======================");
                TextBoxConsole.WriteLine("调用GetText失败！");
                TextBoxConsole.WriteLine(e.Message);
                TextBoxConsole.WriteLine("======================警告======================");
                return src;
            }
            return lastText=p.StandardOutput.ReadToEnd();
        }
        public bool CheckText(string checkstr,string src=null)
        {
            if (src != null)//考虑到一般不需要第二个参数，故默认用lastText做判断
                lastText = src;
            if (lastText.Contains(checkstr))
            {
                TextBoxConsole.WriteLine("===================检测到翻译错误===================");
                TextBoxConsole.WriteLine("错误的字段：" + lastText);
                TextBoxConsole.WriteLine("所在MO文件：" + _TEXTDOMAIN);
                TextBoxConsole.WriteLine("===================检测到翻译错误===================");
                return false;
            }
            return true;
        }
    }
}
