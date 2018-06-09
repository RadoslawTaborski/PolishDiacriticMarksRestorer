using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreLinq;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common.NgramsConnectors
{
    public class Variant1 : INgramsConnector
    {
        public List<string> AnalyzeNgramsVariants(List<NGramVariants> ngramsVar, int length, int countWords)
        {
            var result = new List<string>();

            var finalVariants = FindTheBestNgramFromNgramsVariants(ngramsVar);

            for (var i = 0; i < countWords; ++i)
            {
                var tmp = CreateFrame(finalVariants, length, i);
                result.Add(FindWordFromNgramList(tmp));
            }

            return result;
        }

        private List<NGram> FindTheBestNgramFromNgramsVariants(List<NGramVariants> variants)
        {
            var finalVariants = new List<NGram>();
            foreach (var item in variants)
            {
                var ngram = item.NgramVariants.MaxBy(x => x.Ngram.Value).ElementAt(0);
                finalVariants.Add(ngram.Ngram);
            }

            return finalVariants;
        }

        private List<NGram> CreateFrame(List<NGram> ngrams, int length, int indexOfWord)
        {
            var result = new List<NGram>();

            for (var i = indexOfWord - length + 1; i <= indexOfWord; ++i)
            {
                if (i < 0 || i >= ngrams.Count)
                    result.Add(new NGram(-1, new List<string>()));
                else
                    result.Add(ngrams[i]);
            }

            return result;
        }

        private string FindWordFromNgramList(List<NGram> ngrams)
        {
            var ngram = ngrams.MaxBy(x => x.Value).ElementAt(0);
            var index = ngrams.IndexOf(ngram);

            return ngram.WordsList[ngram.WordsList.Count - index - 1];
        }
    }
}
