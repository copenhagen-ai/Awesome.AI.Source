"""Port of Common/GPTProbability.cs."""
import math


class GPTProbability:
    SQRT2PI = 2.5066282746310002

    def Use(self, _val, _b, mind):
        norm = mind.calc.Normalize(_val, 0.0, 100.0, 0.0, 10.0)
        per = mind.prob.NormalPDF(norm, 5.0, 4.0) * 100.0
        if per > 100.0: per = 100.0
        flip = mind.calc.Chance(100, 100 - int(per))
        return not _b if flip else _b

    def NormalPDF(self, x, mean, stdDev):
        z = (x - mean) / stdDev
        return math.exp(-0.5 * z * z) / (stdDev * self.SQRT2PI)

    def NormalCDF(self, x, mean, stdDev):
        z = (x - mean) / (stdDev * math.sqrt(2))
        return 0.5 * (1 + self.Erf(z))

    def ProbabilityBetween(self, a, b, mean, stdDev):
        return self.NormalCDF(b, mean, stdDev) - self.NormalCDF(a, mean, stdDev)

    def Erf(self, x):
        t = 1.0 / (1.0 + 0.3275911 * abs(x))
        a = (0.254829592, -0.284496736, 1.421413741, -1.453152027, 1.061405429)
        total = t * (a[0] + t * (a[1] + t * (a[2] + t * (a[3] + t * a[4]))))
        result = 1.0 - total * math.exp(-x * x)
        return result if x >= 0 else -result
