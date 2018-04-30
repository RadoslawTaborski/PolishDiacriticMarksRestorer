using System;
using System.Linq;

namespace NgramAnalyzer.Common
{
    public static class TextSpliter
    {
        public static readonly string[] Delimiters = { " ", "\r\n", "\t" };

        public static string[] Split(string text)
        {
            return text.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] SplitAndKeep(string text)
        {
            return text.SplitAndKeep(Delimiters).ToArray();
        }
    }
}
