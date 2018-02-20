using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace PolishDiacriticMarksRestorer
{
    /// <summary>
    /// Settings Static Class with the program settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The type o Ngram.
        /// </summary>
        public static NgramType Type = NgramType.Digram;
        /// <summary>
        /// The server adress.
        /// </summary>
        public static string Server = "localhost";
        /// <summary>
        /// The database name.
        /// </summary>
        public static string DbName = "ngrams";
        /// <summary>
        /// The database user.
        /// </summary>
        public static string DbUser = "root";
        /// <summary>
        /// Use dictionary from file instead database.
        /// </summary>
        public static bool FileDictionary = true;
        /// <summary>
        /// The database password.
        /// </summary>
        public static string DbPassword = "";
        /// <summary>
        /// The table names list.
        /// </summary>
        public static IList<string> TableNames = new []{"dictionary", "2grams", "3grams", "4grams"};
        /// <summary>
        /// The alphabetical tables in database
        /// </summary>
        public static bool AlphabeticalTables = false;
        /// <summary>
        /// Path to dictionary file
        /// </summary>
        public static string DictionaryPath = @"Resources/dictionary";
    }
}
