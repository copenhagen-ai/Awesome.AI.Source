from dataclasses import dataclass
import math


@dataclass
class GPTQubit2Old:
    alpha: complex = 1 + 0j
    beta: complex = 0 + 0j
    def Normalize(self):
        norm = math.sqrt(abs(self.alpha) ** 2 + abs(self.beta) ** 2)
        if norm: self.alpha /= norm; self.beta /= norm
        return self

