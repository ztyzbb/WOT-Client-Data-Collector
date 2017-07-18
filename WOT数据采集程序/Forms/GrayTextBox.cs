using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace WOT数据采集程序
{
    class GrayTextBox : TextBox
    {
        public GrayTextBox()
        {
            ForeColor = SystemColors.GrayText;
        }

        private string _defaultString;
        [DefaultValue(typeof(string), ""), Description("默认字符")]
        public string DefaultString
        {
            get { return _defaultString; }
            set
            {
                _defaultString = value;
                if (ForeColor == SystemColors.GrayText)
                    Text = _defaultString;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (ForeColor == SystemColors.GrayText)
            {
                Text = null;
                ForeColor = SystemColors.WindowText;
            }
        }
        protected override void OnLostFocus(EventArgs e)
        {
            if (Text.Length == 0)
            {
                ForeColor = SystemColors.GrayText;
                Text = _defaultString;
            }
        }
    }
}
