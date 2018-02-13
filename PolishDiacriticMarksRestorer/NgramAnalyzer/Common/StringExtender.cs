using System.Text.RegularExpressions;

namespace NgramAnalyzer.Common
{
    /// <summary>
    /// This class extend string class.
    /// </summary>
    public static class StringExtender
    {
        #region Public
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
        #endregion
    }
}
