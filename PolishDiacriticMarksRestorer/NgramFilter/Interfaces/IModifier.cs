using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IModifier
    {
        /// <summary>
        /// Run all added ModifierItem and edit words if they do not meet the criteria  
        /// </summary>
        /// <param name="ngram"></param>
        /// <returns></returns>
        NGram Start(NGram ngram);
        /// <summary>
        /// Add new ModifierItem to list
        /// </summary>
        /// <param name="item">ModifierItem which will be added</param>
        void Add(IModifierItem item);
        /// <summary>
        /// Size of ModifierItem list
        /// </summary>
        /// <returns>size</returns>
        int Size();
    }
}
