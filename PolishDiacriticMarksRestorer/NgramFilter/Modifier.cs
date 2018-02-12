using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    /// <summary>
    /// Modifier Class stores and runs ModifierItem
    /// </summary>
    internal class Modifier : IModifier
    {
        #region FIELDS
        private readonly List<IModifierItem> _modifiers;
        #endregion

        #region CONSTRUCTORS
        public Modifier()
        {
            _modifiers = new List<IModifierItem>();
        }
        #endregion

        #region  PUBLIC
        /// <inheritdoc />
        public void Add(IModifierItem item)
        {
            _modifiers.Add(item);
        }

        /// <inheritdoc />
        public int Size()
        {
            return _modifiers.Count;
        }

        /// <inheritdoc />
        public NGram Start(NGram ngram)
        {
            foreach (var item in _modifiers)
            {
                ngram = item.Edit(ngram);
            }

            return ngram;
        }
        #endregion

        #region PRIVATE

        #endregion
    }
}
