using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// This class extend string class.
    /// </summary>
    public static class StringExtender
    {
        #region PUBLIC
        /// <summary>
        /// Nth index of string in string.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="value">The search value.</param>
        /// <param name="n">The nth value.</param>
        /// <returns>Index if found, otherwise -1.</returns>
        public static int NthIndexOf(this string target, string value, int n)
        {
            var m = Regex.Match(target, "((" + Regex.Escape(value) + ").*?){" + n + "}");

            if (m.Success)
                return m.Groups[2].Captures[n - 1].Index;
            return -1;
        }

        /// <summary>
        /// Changes the special characters for database.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>changed string</returns>
        public static string ChangeSpecialCharacters(this string target)
        {
            target = target.Replace(@"\", @"\\");
            target = target.Replace(@"'", @"\'");
            return target;
        }

        /// <summary>
        /// Remove punctation marks from string.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>String without punctation marks.</returns>
        public static string WithoutPunctationMarks(this string target)
        {
            target = target.Replace(@",", @"");
            target = target.Replace(@";", @"");
            target = target.Replace(@"!", @"");
            target = target.Replace(@"?", @"");
            target = target.Replace(@"(", @"");
            target = target.Replace(@")", @"");
            target = target.Replace(@"""", @"");
            target = target.Replace(@".", @"");
            target = target.Replace(@":", @"");
            return target;
        }

        public static int IndexOfAny(this string test, string[] anyOf, int startIndex, out string delimiter)
        {
            var first = -1;
            delimiter = "";
            foreach (var item in anyOf)
            {
                var i = test.IndexOf(item, startIndex, StringComparison.Ordinal);
                if (i < 0) continue;
                if (first > 0)
                {
                    if (i >= first) continue;
                    first = i;
                    delimiter = item;
                }
                else
                {
                    first = i;
                    delimiter = item;
                }
            }
            return first;
        }

        public static IEnumerable<string> SplitAndKeep(this string s, string[] delimiters)
        {
            int start = 0, index;
            while ((index = s.IndexOfAny(delimiters, start, out var delimiter)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index - start);
                yield return s.Substring(index, delimiter.Length);
                start = index + delimiter.Length;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }

        public static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", nameof(value));
            var indexes = new List<int>();
            for (var index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.OrdinalIgnoreCase);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        public static string RemoveDiacritics(this string s)
        {
            var rgx = new Regex("[ĄĆĘŁŃÓŚŻŹąćęłńóśżź]");
            if (!rgx.IsMatch(s)) return s;

            var sb = new StringBuilder(s);
            for (var i = 0; i < sb.Length; i++)
            {
                sb[i] = NormalizeChar(sb[i]);
            }

            return sb.ToString();
        }
        #endregion

        #region PRIVATE
        private static char NormalizeChar(char c)
        {
            switch (c)
            {
                case 'ą':
                    return 'a';
                case 'ć':
                    return 'c';
                case 'ę':
                    return 'e';
                case 'ł':
                    return 'l';
                case 'ń':
                    return 'n';
                case 'ó':
                    return 'o';
                case 'ś':
                    return 's';
                case 'ż':
                case 'ź':
                    return 'z';
                case 'Ą':
                    return 'A';
                case 'Ć':
                    return 'C';
                case 'Ę':
                    return 'E';
                case 'Ł':
                    return 'L';
                case 'Ń':
                    return 'N';
                case 'Ó':
                    return 'O';
                case 'Ś':
                    return 'S';
                case 'Ź':
                case 'Ż':
                    return 'Z';
            }

            return c;
        }

        public static string GetLast(this string source, int tailLength)
        {
            return tailLength >= source.Length ? source : source.Substring(source.Length - tailLength);
        }
        #endregion
    }
}
