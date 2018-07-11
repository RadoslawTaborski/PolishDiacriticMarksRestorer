namespace EffectivenessResearch
{
    internal struct ConfusionMatrix
    {
        public int TruePositive;
        public int FalsePositive;
        public int FalseNegative;
        public int TrueNegative;

        public double Sensitivity() => (double)TruePositive / (TruePositive + FalseNegative);
        public double Specificity() => (double)TrueNegative / (FalsePositive + TrueNegative);
        public double Precision() => (double)TruePositive / (TruePositive + FalsePositive);
        public double NegativePredictiveValue() => (double)TrueNegative / (FalseNegative + TrueNegative);
        public double FalseNegativeRate() => (double)FalseNegative / (FalseNegative + TruePositive);
        public double FalsePositiveRate() => (double)FalsePositive / (FalsePositive + TrueNegative);
        public double FalseDiscoveryRate() => (double)FalsePositive / (FalsePositive + TruePositive);
        public double FalseOmissionRate() => (double)FalseNegative / (FalseNegative + TrueNegative);
        public double Accuracy() => (double)(TruePositive + TrueNegative) / (TruePositive + TrueNegative + FalsePositive + FalseNegative);

        public override string ToString()
        {
            return $"\t{TruePositive}\t{FalsePositive}\r\n\t{FalseNegative}\t{TrueNegative}\r\nSensitivity:\t{Sensitivity():F5}\r\nSpecificity:\t{Specificity():F5}\r\nPrecision:\t{Precision():F5}\r\nNegativePredictiveValue:\t{NegativePredictiveValue():F5}\r\nFalseNegativeRate:\t{FalseNegativeRate():F5}\r\nFalsePositiveRate:\t{FalsePositiveRate():F5}\r\nFalseDiscoveryRate:\t{FalseDiscoveryRate():F5}\r\nFalseOmissionRate:\t{FalseOmissionRate():F5}\r\nAccuracy:\t{Accuracy():F5}\r\n";
        }
    }
}
