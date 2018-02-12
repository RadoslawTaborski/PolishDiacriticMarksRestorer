using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.ModifierItems
{
    /// <summary>
    /// RemoveNonPunctationMarks Class removed wrong marks from ngram
    /// </summary>
    internal class RemoveNonPunctationMarks: IModifierItem
    {
        #region FIELDS

        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <inheritdoc />
        public NGram Edit(NGram ngram)
        {
            return ngram;
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}
