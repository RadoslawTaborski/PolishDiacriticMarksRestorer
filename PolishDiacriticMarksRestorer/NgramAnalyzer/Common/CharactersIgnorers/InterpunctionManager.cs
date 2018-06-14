using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace NgramAnalyzer.Common.CharactersIgnorers
{
    public class InterpunctionManager : Interfaces.ICharactersIgnorer
    {
        public NGram Remove(NGram actual)
        {
            var result = new NGram(actual);
            for (var i = 0; i < result.WordsList.Count; i++)
            {
                Regex reg = new Regex("[^a-zA-Z'\\-, ąĄćĆęĘłŁńŃóÓźŹżŻśŚ]");
                result.WordsList[i]=reg.Replace(result.WordsList[i], string.Empty);
            }

            return result;
        }

        public NGram Restore(NGram old, NGram actual)
        {
            var ngram = new NGram(actual);
            for (var i = 0; i < old.WordsList.Count; i++)
            {
                var item = old.WordsList[i];

                for (var index = 0; index < item.Length; index++)
                {
                    var charact = item[index];
                    if (ngram.WordsList[i].Length > index)
                    {
                        if (!charact.Equals(ngram.WordsList[i][index]))
                            ngram.WordsList[i] = ngram.WordsList[i].Insert(index, charact.ToString());
                    }
                    else
                    {
                        ngram.WordsList[i] = ngram.WordsList[i].Insert(index, charact.ToString());
                    }
                }
            }

            return ngram;
        }
    }
}
