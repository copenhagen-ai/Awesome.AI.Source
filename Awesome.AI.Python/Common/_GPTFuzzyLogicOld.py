class GPTFuzzyLogicOld:
    """Legacy compatibility implementation."""
    def Evaluate(self, value): return max(0.0, min(1.0, float(value)))

