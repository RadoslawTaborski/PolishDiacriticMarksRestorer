﻿using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using NgramAnalyzer.Interfaces;

namespace NgramAnalyzer.Common.NgramsConnectors
{
    public class Variant2 : INgramsConnector
    {
        private readonly List<string> _preposition = new List<string>
        {
            "z",
            "ze",
            "za",
            "pod",
            "pode",
            "nad",
            "nade",
            "przed",
            "przede",
            "między",
            "pomiędzy",
            "ja",
            "on",
            "ona",
            "ono"
        };

        public List<string> AnalyzeNgramsVariants(List<NGramVariants> ngramsVar, int length, int countWords)
        {
            var result = new List<string>();

            var finalVariants = new NGram[ngramsVar.Count];
            for (var index = 0; index < ngramsVar.Count; index++)
            {
                if (ngramsVar[index].NgramVariants.Exists(x=>Math.Abs(x.Probability - 1) < 0.000001))
                    finalVariants[index] = new NGram(int.MaxValue, ngramsVar[index].NgramVariants[0].Ngram.WordsList);
            }

            var idx = new List<int>();
            for (var i = 0; i < ngramsVar.Count; ++i)
            {
                foreach (var item in ngramsVar[i].OrginalNGram.WordsList.Take(ngramsVar[i].OrginalNGram.WordsList.Count - 1))
                {
                    if (ngramsVar[i].NgramVariants.Count < 2) continue;
                    var flag = false;
                    foreach (var prep in _preposition)
                    {
                        if (!item.WithoutPunctationMarks().ToLower().Equals(prep)) continue;
                        idx.Add(i);
                        if(i+1<ngramsVar.Count-1)
                            idx.Add(i+1);
                        flag = true;
                        break;
                    }

                    if (flag) break;
                }
            }

            var param2 = 1;
            var sorted = ngramsVar
                .Select((x, i) => new KeyValuePair<double, int>(x.NgramVariants.MaxBy(a => a.Ngram.Value).ElementAt(0).Ngram.Value, i))
                .OrderBy(x => x.Key)
                .ToList();

            var aa = sorted.Where(x => x.Key > param2);
            var keyValuePairs = aa as KeyValuePair<double, int>[] ?? aa.ToArray();
            if (keyValuePairs.Any())
            {
                var ee = keyValuePairs.ElementAt(0);
                var ii = sorted.IndexOf(ee);
                var sublist = sorted.GetRange(0, ii);
                var sublist2 = sorted.GetRange(ii, sorted.Count() - sublist.Count);
                sublist.Reverse();
                sublist2.AddRange(sublist);
                sorted = sublist2;
            }

            var idx2 = sorted.Select(x => x.Value).ToList();
            idx.AddRange(idx2);

            for (var i = 0; i < idx.Count; i++)
            {
                if (finalVariants[idx[i]].WordsList != null) continue;

                var tmp = ngramsVar[idx[i]].NgramVariants;
                if (idx[i] - 1 >= 0)
                    if (finalVariants[idx[i] - 1].WordsList != null)
                    {
                        tmp = tmp.Where(x => x.Ngram.WordsList[0] == finalVariants[idx[i] - 1].WordsList[1]).ToList();
                    }
                if (idx[i] + 1 < ngramsVar.Count && tmp.Count > 1)
                    if (finalVariants[idx[i] + 1].WordsList != null)
                    {
                        tmp = tmp.Where(x => x.Ngram.WordsList[1] == finalVariants[idx[i] + 1].WordsList[0]).ToList();
                    }

                if (tmp.Count > 0)
                {
                    var ngram = tmp.MaxBy(x => x.Ngram.Value).ElementAt(0);
                    finalVariants[idx[i]] = ngram.Ngram;
                }
                else
                {
                    var ngram = ngramsVar[i].NgramVariants.MaxBy(x => x.Ngram.Value).ElementAt(0);
                    finalVariants[idx[i]] = ngram.Ngram;
                }
            }


            for (var i = 0; i < countWords; ++i)
            {
                var tmp = CreateFrame(finalVariants.ToList(), length, i);
                result.Add(FindWordFromNgramList(tmp));
            }

            return result;
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
