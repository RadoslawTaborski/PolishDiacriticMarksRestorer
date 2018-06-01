using System.Collections.Generic;
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
        public static readonly string Server = "localhost";
        /// <summary>
        /// The database name.
        /// </summary>
        public static string DbName = "alphabig";
        /// <summary>
        /// The database user.
        /// </summary>
        public static readonly string DbUser = "root";

        public static bool UseDictionary = true;
        ///// <summary>
        ///// Use dictionary from file instead database.
        ///// </summary>
        //public static bool FileDictionary = true;
        /// <summary>
        /// The database password.
        /// </summary>
        public static readonly string DbPassword = "";
        /// <summary>
        /// The table names list.
        /// </summary>
        public static IList<string> TableNames = new[] { "unigrams", "digrams", "trigrams", "fourgrams" };
        /// <summary>
        /// The alphabetical tables in database
        /// </summary>
        public static bool AlphabeticalTables = true;
        /// <summary>
        /// Path to dictionary file
        /// </summary>
        public static readonly string DictionaryPath = @"Resources/dictionary";
        /// <summary>
        /// Path to unigrams file
        /// </summary>
        public static string UnigramPath = @"Resources/unigrams";

        public static bool SentenceSpliterOn=true;

        public static bool IgnorePunctationMarks = true;
    }
}
