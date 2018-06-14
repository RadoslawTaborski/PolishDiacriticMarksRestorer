using System;
using System.Collections.Generic;
using System.Text;
using NgramAnalyzer.Common;

namespace NgramAnalyzer.Interfaces
{
    public interface ICharactersIgnorer
    {
        NGram Remove(NGram actual);
        NGram Restore(NGram old, NGram actual);
    }
}
