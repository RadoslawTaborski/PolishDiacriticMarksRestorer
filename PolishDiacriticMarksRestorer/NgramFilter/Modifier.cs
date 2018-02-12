using System.Collections.Generic;
using NgramAnalyzer.Common;
using NgramFilter.Interfaces;

namespace NgramFilter
{
    /// <summary>
    /// Modifier Class stores and runs ModifierItem.
    /// </summary>
    /// <seealso cref="NgramFilter.Interfaces.IModifier" />
    internal class Modifier : IModifier
    {
        #region FIELDS
        private readonly List<IModifierItem> _modifiers;
        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a new instance of the <see cref="Modifier"/> class.
        /// </summary>
        public Modifier()
        {
            _modifiers = new List<IModifierItem>();
        }
        #endregion

        #region  PUBLIC
        /// <summary>
        /// Add new ModifierItem to list.
        /// </summary>
        /// <param name="item">ModifierItem which will be added.</param>
        /// <inheritdoc />
        public void Add(IModifierItem item)
        {
            _modifiers.Add(item);
        }

        /// <summary>
        /// Size of ModifierItem list.
        /// </summary>
        /// <returns>
        /// List size.
        /// </returns>
        /// <inheritdoc />
        public int Size()
        {
            return _modifiers.Count;
        }

        /// <summary>
        /// Run all added ModifierItem and edit words if they do not meet the criteria.
        /// </summary>
        /// <param name="ngram">The ngram.</param>
        /// <returns>
        /// Modified ngram.
        /// </returns>
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
