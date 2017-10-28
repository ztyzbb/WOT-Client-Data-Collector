using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace WOTDataCollector
{
    [ToolboxBitmap(typeof(TextBox))]
    class GrayTextBox : TextBox, ISupportInitialize
    {
        public GrayTextBox() { }

        private Color idleForeColor = SystemColors.WindowText;
        [Browsable(true), Category("Appearance"), Description("默认颜色")]
        public Color IdleForeColor
        {
            get { return idleForeColor; }
            set { idleForeColor = value; }
        }

        public void ResetIdleForeColer()
        {
            idleForeColor = SystemColors.WindowText;
        }

        public bool ShouldSerializeIdleForeColer()
        {
            return idleForeColor != SystemColors.WindowText;
        }

        private Color normalForeColor = SystemColors.WindowText;
        [Browsable(true), Category("Appearance"), Description("有内容时的颜色")]
        public Color NormalForeColor
        {
            get { return normalForeColor; }
            set { normalForeColor = value; }
        }

        public void ResetNormalForeColor()
        {
            normalForeColor = SystemColors.WindowText;
        }

        public bool ShouldSerializeNormalForeColor()
        {
            return normalForeColor != SystemColors.WindowText;
        }

        private string defaultText;
        [Browsable(true), DefaultValue(""), Category("Appearance"), Description("默认字符")]
        public string DefaultText
        {
            get { return defaultText; }
            set { defaultText = value; }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (ForeColor == idleForeColor)
                Text = null;
        }
        protected override void OnLostFocus(EventArgs e)
        {
            if (Text.Length == 0)
                Text = defaultText;
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (Text.Length == 0 || Text == defaultText)
                ForeColor = idleForeColor;
            else
                ForeColor = normalForeColor;
        }

        public void BeginInit() { }

        public void EndInit()
        {
            if (Text.Length == 0 || Text == defaultText)
                Text = defaultText;
        }
    }
}
