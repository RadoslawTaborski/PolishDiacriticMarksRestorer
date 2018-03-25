using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PolishDiacriticMarksRestorer
{
    public static class RichTextBoxExtensions
    {
        public static void AppendTextColors(this RichTextBox richTextBox, String word, SolidColorBrush background, SolidColorBrush foreground)
        {
            //Current word at the pointer
            TextRange tr = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd)
            {
                Text = word
            };
            tr.ApplyPropertyValue(TextElement.BackgroundProperty, background);
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, foreground);
        }
    }
}
