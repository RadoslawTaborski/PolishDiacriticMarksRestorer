﻿using System.Collections.Generic;
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
            int first = -1;
            delimiter = "";
            foreach (string item in anyOf)
            {
                int i = test.IndexOf(item, startIndex);
                if (i >= 0)
                {
                    if (first > 0)
                    {
                        if (i < first)
                        {
                            first = i;
                            delimiter = item;
                        }
                    }
                    else
                    {
                        first = i;
                        delimiter = item;
                    }
                }
            }
            return first;
        }

        public static IEnumerable<string> SplitAndKeep(this string s, string[] delimiters)
        {
            int start = 0, index;
            while ((index = s.IndexOfAny(delimiters, start, out string delimiter)) != -1)
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
        #endregion
    }
}
