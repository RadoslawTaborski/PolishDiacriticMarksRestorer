using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter.ModifierItems
{
    /// <summary>
    /// RemoveNonPunctationMarks Class removed wrong marks from ngram.
    /// </summary>
    /// <seealso cref="NgramFilter.Interfaces.IModifierItem" />
    internal class RemoveNonPunctationMarks: IModifierItem
    {
        #region FIELDS

        #endregion

        #region CONSTRUCTORS

        #endregion

        #region  PUBLIC
        /// <summary>
        /// This method edit ngram.
        /// </summary>
        /// <param name="ngram">Ngram which will be edited.</param>
        /// <returns>
        /// Edited ngram.
        /// </returns>
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
