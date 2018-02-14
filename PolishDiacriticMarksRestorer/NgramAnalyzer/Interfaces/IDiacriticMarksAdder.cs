using System.Collections.Generic;

namespace NgramAnalyzer.Interfaces
{
    public interface IDiacriticMarksAdder
    {
        /// <summary>
        /// Adds diacritics to the word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="howManyChanges">How many add operations.</param>
        /// <returns>
        /// List with pair - word - number of operations
        /// </returns>
        List<KeyValuePair<string, int>> Start(string word, int howManyChanges);
    }
}
