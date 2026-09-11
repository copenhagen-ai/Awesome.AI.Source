import math

from awesome_ai.Variables.Constants import CONST


class MyCalc:
    def __init__(self, mind): self.mind = mind
    def Chance(self, count, gt):
        rand = self.mind.rand.MyRandomInt(1, count)[0]
        return rand > count - gt
    def IsRandomSample(self, count, gt): return self.Chance(count, gt)
    def Normalize(self, val, valmin, valmax, ranmin=0.0, ranmax=1.0):
        if valmax == valmin: raise ValueError("valmin and valmax must differ")
        return ranmin + (val - valmin) * (ranmax - ranmin) / (valmax - valmin)
    def Denormalize(self, normalized, minimum, maximum): return minimum + normalized * (maximum - minimum)
    def ToPercent1(self, _this, _from): return self.Normalize(_this, 0.0, _from) * 100.0
    def ToPercent2(self, _this, _from): return _this / _from * 100.0
    def RoundOff(self, i): return int(round(i / 10.0) * 10)
    def Round(self, num, dec, up):
        scale = 10 ** dec
        return (math.ceil(num * scale) if up else math.floor(num * scale)) / scale
    def RoundDouble(self, num, dec): return round(num, dec)
    def RoundInt(self, num): return int(round(num))
    def RoundUp(self, num): return math.ceil(num)
    def RoundDown(self, num): return math.floor(num)
    def HighestIndex(self, arr): return max(range(len(arr)), key=arr.__getitem__) if arr else -1
    def Reciprocal(self, x):
        if x <= 0.0: raise RuntimeError("Reciprocal")
        return 1.0 / x
    def EventHorizon(self, r):
        if r <= 0.0: raise RuntimeError("EventHorizon")
        r = max(r, CONST.RS)
        return math.sqrt(1.0 - CONST.RS / r)
    def Linear(self, x, a, b):
        if x < 0.0: raise RuntimeError("Linear")
        return a * x + b
    def Quadratic(self, x, a, b, c): return a * x * x + b * x + c
    def Logistic(self, x): return 1 / (1 + math.exp(-x))
    def Pyth2D(self, *args):
        if len(args) == 2:
            if args[0] <= 0.0 or args[1] <= 0.0: raise RuntimeError("Pyth")
            return math.hypot(*args)
        if len(args) == 4: return math.hypot(args[0] - args[1], args[2] - args[3])
        raise TypeError("Pyth2D expects 2 or 4 arguments")
    def Pyth6D(self, *args):
        if len(args) != 12: raise TypeError("Pyth6D expects 12 arguments")
        return math.sqrt(sum((args[i] - args[i + 1]) ** 2 for i in range(0, 12, 2)))
    def PythNear2D(self, theta, hypotenuse):
        if theta > 180.0 or theta < -180.0 or hypotenuse <= 0.0: raise RuntimeError("PythNear")
        return math.cos(self.ToRadiansFromDegrees(theta)) * hypotenuse
    def PythFar2D(self, theta, hypotenuse):
        if theta >= 90.0 or theta <= 0.0 or hypotenuse <= 0.0: raise RuntimeError("PythFar")
        return math.sin(self.ToRadiansFromDegrees(theta)) * hypotenuse
    def ToRadiansFromDegrees(self, angle): return math.radians(angle)
    def ToDegreesFromRadians(self, radians): return math.degrees(radians)
    def ToDegreesFromSlope(self, slope): return math.degrees(math.atan(slope))
    def SlopeCoefficient(self, x, a, b): return 2 * a * x + b
    def Roots(self, _x, _a, _b, _c):
        discriminant = _b * _b - 4 * _a * _c
        root = math.sqrt(discriminant)
        x1 = (-_b + root) / (2 * _a)
        x2 = (-_b - root) / (2 * _a)
        if _x is None: return -1.0
        return x2 if _x >= 0.0 else x1
