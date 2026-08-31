from __future__ import annotations
from dataclasses import dataclass, fields
import math


@dataclass
class GPTVector6D:
    xx: float = 0.0
    yy: float = 0.0
    zz: float = 0.0
    ww: float = 0.0
    vv: float = 0.0
    uu: float = 0.0

    def _values(self): return tuple(getattr(self, f.name) for f in fields(self))
    @classmethod
    def _from(cls, values): return cls(*values)
    def Add(self, v1, v2): return self._from(a + b for a, b in zip(v1._values(), v2._values()))
    def Sub(self, v1, v2): return self._from(a - b for a, b in zip(v1._values(), v2._values()))
    def Mul(self, vector, scalar): return self._from(a * scalar for a in vector._values())
    def Div(self, vector, scalar):
        if scalar == 0: raise ZeroDivisionError("scalar")
        return self._from(a / scalar for a in vector._values())
    def Dot(self, v1, v2): return sum(a * b for a, b in zip(v1._values(), v2._values()))
    @property
    def magnitude(self): return math.sqrt(self.Dot(self, self))
    def Unit(self):
        length = self.magnitude
        if length == 0: raise ZeroDivisionError("Cannot normalize a zero vector.")
        return self.Div(self, length)
    def Reverse(self): return self._from(100.0 - value for value in self._values())
    def ReverseUnit(self): return self._from(-value for value in self._values())
    def DistanceTo(self, other): return self.Sub(self, other).magnitude
