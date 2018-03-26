using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PolishDiacriticMarksRestorer
{
    public static class RichTextBoxExtensions
    {
        public static void AppendTextColors(this RichTextBox richTextBox, string word, SolidColorBrush background, SolidColorBrush foreground)
        {
            //Current word at the pointer
            var tr = new TextRange(richTextBox.Document.ContentEnd, richTextBox.Document.ContentEnd)
            {
                Text = word
            };
            tr.ApplyPropertyValue(TextElement.BackgroundProperty, background);
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, foreground);
        }
    }
}
