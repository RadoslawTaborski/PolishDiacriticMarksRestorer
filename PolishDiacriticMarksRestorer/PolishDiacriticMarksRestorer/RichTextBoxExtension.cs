using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using NgramAnalyzer.Common;

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

        public static int Bound { get; set; } = 14;
        private static readonly char[] WhiteChars = { ' ', '\n', '\t', '\r' };

        public static TextRange GetDocument(this RichTextBox rtb)
        {
            return new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        }

        public static void SetCarretPosiotion(this RichTextBox rtb, Point point)
        {
            var caretPos = rtb.GetPositionFromPoint(point, true);
            rtb.CaretPosition = caretPos;
        }

        // zwraca słowo na którym jest aktualnie kursor
        public static TextRange GetSelectedWord(this RichTextBox rtb)
        {
            const int bound = 14;
            var x = rtb.Document.ContentStart.GetOffsetToPosition(rtb.CaretPosition);
            var toEnd = -rtb.Document.ContentEnd.GetOffsetToPosition(rtb.CaretPosition);

            var start = x < bound ? 0 : x - bound;
            var end = toEnd > bound ? x + bound : x + toEnd;

            var point = rtb.Document.ContentStart;

            var s1 = new TextRange(point.GetPositionAtOffset(start), point.GetPositionAtOffset(x)).Text;
            var s2 = new TextRange(point.GetPositionAtOffset(x), point.GetPositionAtOffset(end)).Text;


            // ******************************************************************************************

            var l = -1;
            for (var i = s1.Length - 1; i >= 0; --i)
                if (IsWhiteChar(s1[i]))
                {
                    l = i;
                    break;
                }
            if (l != -1)
                s1 = s1.Remove(0, l + 1);

            var r = -1;
            for (var i = 0; i < s2.Length; ++i)
                if (IsWhiteChar(s2[i]))
                {
                    r = i;
                    break;
                }
            if (r != -1)
                s2 = s2.Remove(r);

            var tr = new TextRange(point.GetPositionAtOffset(x - s1.Length), point.GetPositionAtOffset(x + s2.Length));

            return tr;
        }

        public static void SetContextMenu(this RichTextBox rtb, List<string> words, Regex rgx, SolidColorBrush f1, SolidColorBrush b1, SolidColorBrush f2, SolidColorBrush b2)
        {

            rtb.RemoveContextMenu();
            rtb.ContextMenu?.Items.Add(new Separator());
            foreach (var word in words)
            {
                rtb.ContextMenu?.Items.Add(rgx.IsMatch(word)
                    ? CreateMenuItem(word, rtb, f1, b1)
                    : CreateMenuItem(word, rtb, f2, b2));
            }
        }

        public static void RemoveContextMenu(this RichTextBox rtb)
        {
            var items = rtb.ContextMenu?.Items;
            if (!(items?.Count > 1)) return;
            for (var i = items.Count - 1; i > 0; --i)
                items.RemoveAt(i);
        }

        private static MenuItem CreateMenuItem(string word, RichTextBox rtb, SolidColorBrush foreground, SolidColorBrush background)
        {
            var m = new MenuItem
            {
                Header = word,
                Tag = rtb,
            };
            m.Click += (sender, e) => ChangeWord(e, foreground, background);
            return m;
        }

        private static void ChangeWord(RoutedEventArgs e, SolidColorBrush foreground, SolidColorBrush background)
        {
            var m = (MenuItem)e.OriginalSource;
            var rtb = (RichTextBox)m.Tag;
            var word = rtb.GetSelectedWord();
            word.Text = m.Header.ToString();
            word.ApplyPropertyValue(TextElement.BackgroundProperty, background);
            word.ApplyPropertyValue(TextElement.ForegroundProperty, foreground);
        }

        public static void ColorFont(this TextRange tr, SolidColorBrush color)
        {
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private static bool IsWhiteChar(char c)
        {
            return WhiteChars.Any(character => c == character);
        }

        public static List<TextRange> FindWord(this RichTextBox rtb, string word)
        {
            var position = rtb.Document.ContentStart;
            var words = new List<TextRange>();

            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    var indexInRun = textRun.AllIndexesOf(word);
                    if (indexInRun.Count != 0)
                    {
                        foreach (var index in indexInRun)
                        {
                            var start = position.GetPositionAtOffset(index);
                            if (start == null) continue;
                            var end = start.GetPositionAtOffset(word.Length);
                            words.Add(new TextRange(start, end));
                        }
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "word" is not found.
            return words;
        }

        public static List<string> GetWords(this RichTextBox rtb)
        {
            var wordsList = new List<string>();

            var text = rtb.GetDocument().Text;
            var words = text.Split(WhiteChars);

            foreach (var w in words)
            {
                var ww = w.Trim();
                if (ww.Length > 1 && !wordsList.Contains(ww))
                    wordsList.Add(ww);
            }

            return wordsList;
        }
    }
}
