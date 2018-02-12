using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Settings Static Class with the program settings 
    /// </summary>
    public static class Settings
    {
        public static NgramType Type = NgramType.Digram;
        public static string Server = "localhost";
        public static string DbName = "ngrams";
        public static string DbUser = "root";
        public static string DbPassword = "";
        public static IList<string> TableNames = new []{"1grams", "2grams", "3grams", "4grams"};
    }
}
