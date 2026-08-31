"""Python equivalents of the C# ``MyExtensions`` extension methods."""
from __future__ import annotations
import math
import random
import time
from enum import Enum
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import HACKMODE, MINDS


_rng = random.Random()


def BusyWait(txt, count, mind):
    if mind.epochs > CONST.FIRST_RUN * 3: print(f" : {txt}", end="")
    for _ in range(max(0, count)): time.sleep(0.01)


def RandomSample(count, mind): return mind.calc.Chance(count, 10)


def Shuffle(values):
    for index in range(len(values) - 1, 0, -1):
        swap = _rng.randrange(index + 1)
        values[index], values[swap] = values[swap], values[index]


def ToEnum(value, enum_type):
    if not issubclass(enum_type, Enum): raise TypeError("enum_type must be an Enum")
    return enum_type[str(value).upper()]


def IsNull(source): return source is None
def IsNullOrEmpty(source): return source is None or len(source) == 0


def HasValue(source):
    if isinstance(source, float): return not math.isnan(source) and not math.isinf(source)
    return source is not None and source != ""


def Convert(_x, mind): return mind.calc.Normalize(_x, 0.0, 100.0, CONST.MIN, CONST.MAX)


def Index(value, mind=None):
    if hasattr(value, "UIget"):
        index = int(value.UIget("will"))
        for level in range(9, -1, -1):
            if index > level * 10: return (level + 1) * 10
        raise RuntimeError("Extensions, Index")
    if mind is None: raise ValueError("mind is required for a numeric mood index")
    value = mind.calc.Normalize(value, 10.0, 90.0, 0.0, 100.0)
    return "0" if value < 10.0 else str(value)[0]


def Sign(val): return -1.0 if val < 0.0 else 1.0


def Flip(_x, mind):
    low, high = mind.mech.ms.dv_sym_low, mind.mech.ms.dv_sym_high
    _result = mind.calc.Normalize(_x, low, high, -1.0, 1.0) * -1.0
    return mind.calc.Normalize(_x, -1.0, 1.0, low, high)


def HighZero(val): return 100.0 - val
def LowZero(val): return val
def Roberta(mind): return mind.mindtype == MINDS.ROBERTA
def Andrew(mind): return mind.mindtype == MINDS.ANDREW
def Basic(mind): return mind.mindtype == MINDS.BASIC
def TheHack(_b, mind): return not _b if CONST.hack == HACKMODE.HACK else _b
