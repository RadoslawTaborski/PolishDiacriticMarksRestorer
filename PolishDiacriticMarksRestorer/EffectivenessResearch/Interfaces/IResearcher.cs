using System.Collections.Generic;

namespace EffectivenessResearch.Interfaces
{
    interface IResearcher
    {
        List<ClassificationError> Errors { get; }
        ConfusionMatrix Count();
    }
}
