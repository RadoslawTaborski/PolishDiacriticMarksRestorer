using System;
using System.Collections.Generic;
using System.Text;
using NgramAnalyzer.Common;

namespace EffectivenessResearch
{
    /// <summary>
    /// Settings Static Class with the program settings.
    /// </summary>
    internal static class Settings
    {
        /// <summary>
        /// The type o Ngram.
        /// </summary>
        public static NgramType Type = NgramType.Bigram;
        /// <summary>
        /// The server adress.
        /// </summary>
        public static string Server = "localhost";
        /// <summary>
        /// The database name.
        /// </summary>
        public static string DbName = "alphabig";
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
        public static IList<string> TableNames = new[] { "dictionary", "digrams", "trigrams", "fourgrams" };
        /// <summary>
        /// The alphabetical tables in database
        /// </summary>
        public static bool AlphabeticalTables = true;
        /// <summary>
        /// Path to dictionary file
        /// </summary>
        public static string DictionaryPath = @"Resources/dictionary";
    }
}
