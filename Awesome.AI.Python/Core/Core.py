"""Core control and statistics ported from ``Core/Core.cs``."""
from __future__ import annotations

from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import MECHANICS, SELECTACTUAL, STATE


class Core:
    def __init__(self, mind, rand1=0, rand2=0, rand3=0):
        self.mind = mind
        self.stats = mind.stats
        self.history = []
        self.remember = []
        self.hits = {index * 10: 0 for index in range(1, 11)}
        self.units = {index * 10: 0 for index in range(1, 11)}
        available = mind.access.UNITS_ALL()
        self.history.extend(available[index] for index in (rand1, rand2, rand3))
        mind.unit_actual = self.history[1]

    def OK(self, old):
        if self.mind.bot.mech_low in (
            MECHANICS.TUGOFWAR_LOW,
            MECHANICS.BALLONHILL_LOW,
            MECHANICS.CIRCUIT_2_LOW,
        ):
            return self.ReciprocalOK(self.mind.mech.PosXY())
        raise RuntimeError("Core, OK")

    def ReciprocalOK(self, pos):
        try:
            epsilon = 0.0 if self.mind.goodbye else CONST.EPSILON2
            pain_truth_something = self.mind.calc.Reciprocal(pos + epsilon)
            if pain_truth_something > CONST.MAX_PAIN_TRUTH_SOMETHING:
                raise RuntimeError("ReciprocalOK")
            return True, pain_truth_something
        except (ArithmeticError, RuntimeError, ZeroDivisionError):
            return False, CONST.MAX_PAIN_TRUTH_SOMETHING

    def EventHorizonOK(self, pos):
        try:
            time = self.mind.calc.EventHorizon(pos)
            if time <= 0.0: raise RuntimeError("EventHorizonOK")
            return True, time
        except (ArithmeticError, RuntimeError):
            return False, 0.0

    def StopCondition(self):
        if self.mind.epochs >= 60 * self.mind.bot.RUNTIME:
            self.mind.theanswer.data = "It does not"
        self.mind.goodbye = self.mind.theanswer.data == "It does not"

    def UpdateCredit(self):
        current = self.mind.unit_current
        for unit in self.mind.access.UNITS_ALL():
            if unit is None or not unit.Root or unit.Root == current.Root:
                continue
            unit.credits = min(CONST.MAX_CREDIT, unit.credits + CONST.UPD_CREDIT)
        current.credits = max(0.0, current.credits - 1.0)

    def History(self):
        current = self.mind.unit_current
        if current.IsIDLE() or self.mind.STATE == STATE.QUICKDECISION:
            return
        if current.IsQDECISION() or current.IsDECISION():
            return
        self.history.insert(0, current)
        del self.history[CONST.HIST_TOTAL:]

    def ActualUnit(self, _pro):
        if not _pro: return
        if CONST.select_act != SELECTACTUAL.DOMINANT:
            raise NotImplementedError("Core, ActualUnit")
        self.mind.unit_actual = max(self.history, key=self.history.count)

    def Stats(self, _pro):
        if not _pro or self.mind.unit_actual is None:
            return
        if not self.mind.unit_actual.Root:
            return
        if self.mind.STATE == STATE.QUICKDECISION:
            return
        self.Hits()
        self.Units()

    @staticmethod
    def _index(unit):
        value = int(unit.UIget("will"))
        for index in range(9, -1, -1):
            if value > index * 10:
                return (index + 1) * 10
        return 10

    def Hits(self):
        index = self._index(self.mind.unit_actual)
        self.hits[index] += 1
        self.remember.insert(0, index)
        if len(self.remember) > CONST.REMEMBER:
            self.hits[self.remember.pop()] -= 1
        self.mind.stats.hits = dict(self.hits)

    def Units(self):
        self.units = {index * 10: 0 for index in range(1, 11)}
        for unit in self.mind.access.UNITS_ALL():
            self.units[self._index(unit)] += 1
        self.mind.stats.units = dict(self.units)
