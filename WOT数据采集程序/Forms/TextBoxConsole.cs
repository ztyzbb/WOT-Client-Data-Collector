﻿using System;
using System.Windows.Forms;


namespace WOTDataCollector
{
    class TextBoxConsole
    {
        private static TextBox _textBox;
        public static void SetTextBox(TextBox textBox)
        {
            _textBox = textBox;
        }
        public static void Write(string text)
        {
            _textBox.AppendText(text);
        }
        public static void WriteLine(string text)
        {
            _textBox.AppendText(text+ Environment.NewLine);
        }
    }
}
