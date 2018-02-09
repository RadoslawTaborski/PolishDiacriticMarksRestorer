using System;
using System.Collections.Generic;
using System.Text;
using NgramAnalyzer.Common;

namespace NgramFilter.Interfaces
{
    public interface IModifier
    {
        NGram Start(NGram ngram);
        void Add(IModifierItem item);
        int Size();
    }
}
