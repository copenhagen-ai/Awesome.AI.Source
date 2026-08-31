"""Port of Common/MyNormalize.cs extension methods."""


def Norm0(_x, mind, _l, _h): return mind.calc.Normalize(_x, _l, _h, -1.0, 1.0)
def Norm1(_x, mind, _l=-1.0, _h=-1.0): return mind.calc.Normalize(_x, _l, _h, 0.0, 1.0)
def Norm100(_x, mind, _l, _h): return mind.calc.Normalize(_x, _l, _h, 0.0, 100.0)


def _bounds(mind, prefix):
    low = getattr(mind.mech.ms, f"{prefix}_sym_low")
    high = getattr(mind.mech.ms, f"{prefix}_sym_high")
    if low == high: low -= 0.1
    return low, high


def _normal(_x, mind, prefix, range_low, range_high):
    low, high = _bounds(mind, prefix)
    return mind.calc.Normalize(_x, low, high, range_low, range_high)


def Norm0DV(_x, mind): return _normal(_x, mind, "dv", -1.0, 1.0)
def Norm1DV(_x, mind): return _normal(_x, mind, "dv", 0.0, 1.0)
def Norm100DV(_x, mind): return _normal(_x, mind, "dv", 0.0, 100.0)
def Norm0VV(_x, mind): return _normal(_x, mind, "vv", -1.0, 1.0)
def Norm1VV(_x, mind): return _normal(_x, mind, "vv", 0.0, 1.0)
def Norm100VV(_x, mind): return _normal(_x, mind, "vv", 0.0, 100.0)
def Norm0FNET(_x, mind): return _normal(_x, mind, "fnet", -1.0, 1.0)
def Norm1FNET(_x, mind): return _normal(_x, mind, "fnet", 0.0, 1.0)
def Norm100FNET(_x, mind): return _normal(_x, mind, "fnet", 0.0, 100.0)
def Norm0MOM(_x, mind): return _normal(_x, mind, "mom", -1.0, 1.0)
def Norm1MOM(_x, mind): return _normal(_x, mind, "mom", 0.0, 1.0)
def Norm100MOM(_x, mind): return _normal(_x, mind, "mom", 0.0, 100.0)
def Norm0ACC(_x, mind): return _normal(_x, mind, "acc", -1.0, 1.0)
def Norm1ACC(_x, mind): return _normal(_x, mind, "acc", 0.0, 1.0)
def Norm100ACC(_x, mind): return _normal(_x, mind, "acc", 0.0, 100.0)
def Norm0KE(_x, mind): return _normal(_x, mind, "ke", -1.0, 1.0)
def Norm1KE(_x, mind): return _normal(_x, mind, "ke", 0.0, 1.0)
def Norm100KE(_x, mind): return _normal(_x, mind, "ke", 0.0, 100.0)


class MyNormalize:
    Norm0 = staticmethod(Norm0); Norm1 = staticmethod(Norm1); Norm100 = staticmethod(Norm100)
    Norm0DV = staticmethod(Norm0DV); Norm1DV = staticmethod(Norm1DV); Norm100DV = staticmethod(Norm100DV)
    Norm0VV = staticmethod(Norm0VV); Norm1VV = staticmethod(Norm1VV); Norm100VV = staticmethod(Norm100VV)
    Norm0FNET = staticmethod(Norm0FNET); Norm1FNET = staticmethod(Norm1FNET); Norm100FNET = staticmethod(Norm100FNET)
    Norm0MOM = staticmethod(Norm0MOM); Norm1MOM = staticmethod(Norm1MOM); Norm100MOM = staticmethod(Norm100MOM)
    Norm0ACC = staticmethod(Norm0ACC); Norm1ACC = staticmethod(Norm1ACC); Norm100ACC = staticmethod(Norm100ACC)
    Norm0KE = staticmethod(Norm0KE); Norm1KE = staticmethod(Norm1KE); Norm100KE = staticmethod(Norm100KE)

    @staticmethod
    def Normalize(value, minimum, maximum, range_min=0.0, range_max=1.0):
        if maximum == minimum: raise ValueError("minimum and maximum must differ")
        return range_min + (value - minimum) * (range_max - range_min) / (maximum - minimum)

    @staticmethod
    def Denormalize(value, minimum, maximum): return minimum + value * (maximum - minimum)
