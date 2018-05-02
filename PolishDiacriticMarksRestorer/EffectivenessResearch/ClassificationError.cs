using System;
using System.Collections.Generic;
using System.Text;

namespace EffectivenessResearch
{
    public struct ClassificationError
    {
        public string Type;
        public string Original;
        public string Input;
        public string Output;

        public override string ToString()
        {
            return $"{Type}:\t{Original}\t{Input}\t{Output}";
        }
    }
}
