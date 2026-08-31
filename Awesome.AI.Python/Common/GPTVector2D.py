from __future__ import annotations
import math


class GPTVector2D:
    def __init__(self, x=0.0, y=0.0, magnitude=None, radians=None):
        self.xx = math.nan if x is None else float(x)
        self.yy = math.nan if y is None else float(y)
        self.magnitude = math.nan if magnitude is None else float(magnitude)
        self.theta_in_radians = math.nan if radians is None else float(radians)

    @property
    def theta_in_degrees(self) -> float:
        return self.ToDegrees(self)

    def Add(self, v1, v2): return GPTVector2D(v1.xx + v2.xx, v1.yy + v2.yy, None, None)
    def Sub(self, v1, v2): return GPTVector2D(v1.xx - v2.xx, v1.yy - v2.yy, None, None)
    def Mul(self, v1, scalar): return GPTVector2D(v1.xx * scalar, v1.yy * scalar, None, None)
    def Div(self, v1, scalar):
        if scalar == 0: raise ZeroDivisionError("Cannot divide by zero.")
        return GPTVector2D(v1.xx / scalar, v1.yy / scalar, None, None)
    def Dot(self, v1, v2): return v1.xx * v2.xx + v1.yy * v2.yy
    def Unit(self):
        length = math.hypot(self.xx, self.yy)
        if length == 0: raise ZeroDivisionError("Cannot normalize a zero vector.")
        return GPTVector2D(self.xx / length, self.yy / length, 1.0, None)
    def Reverse(self): return GPTVector2D(100.0 - self.xx, 100.0 - self.yy, None, None)
    def ReverseUnit(self): return GPTVector2D(-self.xx, -self.yy, None, None)
    def ToRadians(self, angle): return math.radians(angle)
    def ToDegrees(self, v1): return math.degrees(v1.theta_in_radians)
    def ToPolar(self, v1): return GPTVector2D(v1.xx, v1.yy, math.hypot(v1.xx, v1.yy), math.atan2(v1.yy, v1.xx))
    def ToCart(self, v1): return GPTVector2D(v1.magnitude * math.cos(v1.theta_in_radians), v1.magnitude * math.sin(v1.theta_in_radians), v1.magnitude, v1.theta_in_radians)
