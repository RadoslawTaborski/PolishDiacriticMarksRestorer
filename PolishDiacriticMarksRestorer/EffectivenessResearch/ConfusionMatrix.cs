namespace EffectivenessResearch
{
    internal struct ConfusionMatrix
    {
        public int TruePositive;
        public int FalsePositive;
        public int FalseNegative;
        public int TrueNegative;

        public double Sensitivity() => (double)TruePositive / (TruePositive + TrueNegative);
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
            return $"{TruePositive}\t{FalsePositive}\n\r{FalseNegative}\t{TrueNegative}";
        }
    }
}
