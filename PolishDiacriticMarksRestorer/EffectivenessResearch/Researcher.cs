using System;
using System.Collections.Generic;
using EffectivenessResearch.Interfaces;

namespace EffectivenessResearch
{
    internal class Researcher : IResearcher
    {
        private readonly List<string> _orginalText;
        private readonly List<string> _inputText;
        private readonly List<string> _outputText;

        public Researcher(List<string> orginalText, List<string> inputText, List<string> outputText)
        {
            _orginalText = orginalText;
            _outputText = outputText;
            _inputText = inputText;
        }

        public ConfusionMatrix Count()
        {
            var matrix = new ConfusionMatrix();
            if (_orginalText.Count != _inputText.Count || _inputText.Count != _outputText.Count) throw new ArgumentException("One or more texts has wrong size");

            for (var i = 0; i < _orginalText.Count; i++)
            {
                if (_inputText[i] == _orginalText[i] && _orginalText[i] == _outputText[i]) matrix.TrueNegative++;
                if (_inputText[i] != _orginalText[i] && _orginalText[i] == _outputText[i]) matrix.TruePositive++;
                if (_inputText[i] == _orginalText[i] && _orginalText[i] != _outputText[i])
                {
                    matrix.FalsePositive++;
                    Console.WriteLine($"FP: {_orginalText[i]} {_inputText[i]} {_outputText[i]}");
                }
                if (_inputText[i] != _orginalText[i] && _orginalText[i] != _outputText[i])
                {
                    matrix.FalseNegative++;
                    Console.WriteLine($"FN: {_orginalText[i]} {_inputText[i]} {_outputText[i]}");
                }
            }

            return matrix;
        }
    }
}
