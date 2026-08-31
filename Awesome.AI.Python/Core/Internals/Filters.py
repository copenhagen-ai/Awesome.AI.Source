from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import ORDER, STATE


class Filters:
    def __init__(self, mind): self.mind = mind
    def Valid(self, unit): return unit.IsValid
    def LowCut(self, unit, axis):
        if unit is None: raise ValueError("unit")
        if self.mind.STATE == STATE.QUICKDECISION or axis != CONST.AXES[0]: return True
        low_unit = self.mind.access.UNITS_ALL(ORDER.BYINDEX)[CONST.LOWCUT]
        return unit.UIget("will") > low_unit.UIget("will")
    def Credits(self, unit, axis):
        if unit is None: raise ValueError("unit")
        if self.mind.STATE == STATE.QUICKDECISION or axis != CONST.AXES[0]: return True
        return unit.credits > CONST.LOW_CREDIT
