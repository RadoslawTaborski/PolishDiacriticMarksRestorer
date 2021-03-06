﻿using System.Collections.Generic;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface IFragmentsSplitter
    {
        List<Sentence> Split(List<string> text);
    }
}
