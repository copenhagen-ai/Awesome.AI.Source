"""Current-unit selection and corridors ported from ``USSoup.cs``."""
import math

from awesome_ai.Common import GPTVector2D, GPTVector6D
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import SELECTCURRENT, STATE
from .Unit import UNIT


class GPT:
    @staticmethod
    def Create(): return GPT()

    @staticmethod
    def _vector2(unit):
        return GPTVector2D(unit.UIget(CONST.AXES[0]), unit.UIget(CONST.AXES[1]), None, None)

    @staticmethod
    def _vector6(unit):
        return GPTVector6D(*(unit.UIget(axis) for axis in CONST.AXES))

    def Corridor2D(self, all_units, unit_a, unit_b):
        if unit_b is None:
            return []
        width = 5.0
        func = GPTVector2D()
        p1, p2 = self._vector2(unit_a), self._vector2(unit_b)
        direction = func.Sub(p2, p1)
        length = math.hypot(direction.xx, direction.yy)
        if length == 0:
            return []
        direction = func.Div(direction, length)
        perpendicular = GPTVector2D(-direction.yy, direction.xx, None, None)
        corridor = []
        for unit in all_units:
            relative = func.Sub(self._vector2(unit), p1)
            along, across = func.Dot(relative, direction), func.Dot(relative, perpendicular)
            if 0 <= along <= length and abs(across) <= width / 2:
                corridor.append((along, unit))
        corridor = [unit for _, unit in sorted(corridor, key=lambda item: item[0], reverse=True)]
        self.Fix2D(corridor, unit_a, unit_b)
        if not corridor:
            raise RuntimeError("USSoup, Corridor")
        return corridor

    def Corridor6D(self, all_units, unit_a, unit_b):
        if unit_b is None:
            return []
        width = 5.0
        func = GPTVector6D()
        p1, p2 = self._vector6(unit_a), self._vector6(unit_b)
        direction = func.Sub(p2, p1)
        length = direction.magnitude
        if length == 0:
            return []
        direction = func.Div(direction, length)
        corridor = []
        for unit in all_units:
            relative = func.Sub(self._vector6(unit), p1)
            along = func.Dot(relative, direction)
            closest_offset = func.Mul(direction, along)
            across = func.Sub(relative, closest_offset).magnitude
            if 0 <= along <= length and across <= width / 2:
                corridor.append((along, unit))
        corridor = [unit for _, unit in sorted(corridor, key=lambda item: item[0], reverse=True)]
        self.Fix6D(corridor, unit_a, unit_b)
        if not corridor:
            raise RuntimeError("USSoup, Corridor")
        return corridor

    def Fix2D(self, corridor, unit_a, unit_b):
        while unit_a in corridor: corridor.remove(unit_a)
        while unit_b in corridor: corridor.remove(unit_b)
        corridor.insert(0, unit_b)
        corridor.append(unit_a)

    def Fix6D(self, corridor, unit_a, unit_b):
        self.Fix2D(corridor, unit_a, unit_b)


class Select:
    def __init__(self, mind, soup): self.mind, self.soup = mind, soup
    @staticmethod
    def Create(mind, soup): return Select(mind, soup)
    def ByPyth2D(self, units, near):
        result, minimum = None, 1e21
        for unit in units:
            if unit is self.mind.unit_current: continue
            nearest = self.soup.Near2(unit)
            distance = self.mind.calc.Pyth2D(near.xx, nearest.xx, near.yy, nearest.yy)
            if distance < minimum: result, minimum = unit, distance
        return result

    def ByPyth6D(self, units, near):
        result, minimum = None, 1e21
        for unit in units:
            if unit is self.mind.unit_current: continue
            nearest = self.soup.Near6(unit)
            distance = self.mind.calc.Pyth6D(
                near.xx, nearest.xx, near.yy, nearest.yy, near.zz, nearest.zz,
                near.ww, nearest.ww, near.vv, nearest.vv, near.uu, nearest.uu)
            if distance < minimum: result, minimum = unit, distance
        return result

    def ByOther(self, units, near): raise NotImplementedError("USSoup, SelectOther")


class USSoup:
    def __init__(self, mind): self.mind = mind

    def Quick(self, _pro):
        if not _pro or self.mind.STATE == STATE.QUICKDECISION:
            return False
        if not self.mind.calc.Chance(CONST.SAMPLE50, 10):
            return False

        # ARC quick units were deliberately excluded from TheMind; retain the
        # C# quick-selection mechanism for the three remaining quick units.
        quick_units = (self.mind.q_u_whistle, self.mind.q_u_mathlearn, self.mind.q_u_mathsolve)
        index = self.mind.rand.MyRandomInt(1, 49)[0]
        self.mind.unit_current = quick_units[index % len(quick_units)]
        return True

    def CurrentUnit(self, _pro):
        if self.Quick(_pro): return
        current = self.mind.unit_current
        still_quick = current.IsQDECISION() and self.mind.STATE == STATE.JUSTRUNNING
        if still_quick or current.IsIDLE(): result = self.Buffer()
        else: result = self.Unit()
        self.mind.unit_current, self.mind.unit_corridor = result[0], result

    def Unit(self):
        units = [unit for unit in self.mind.access.UNITS_ALL()
                 if self.mind.filters.Valid(unit) and self.mind.filters.LowCut(unit, "will")
                 and self.mind.filters.Credits(unit, "will")]
        if not units: return [UNIT.CreateIdle(self.mind)]

        near2, near6 = self.Near2(self.mind.unit_current), self.Near6(self.mind.unit_current)
        selected, corridor = None, [self.mind.unit_current]
        selector = Select.Create(self.mind, self)
        if CONST.select_curr == SELECTCURRENT.PYTH2:
            selected = selector.ByPyth2D(units, near2)
            if selected is not None:
                corridor = GPT.Create().Corridor2D(units, self.mind.unit_current, selected)
                corridor[0].Update2D(near2)
        elif CONST.select_curr == SELECTCURRENT.PYTH6:
            selected = selector.ByPyth6D(units, near6)
            if selected is not None:
                corridor = GPT.Create().Corridor6D(units, self.mind.unit_current, selected)
                corridor[0].Update6D(near6)
        elif CONST.select_curr == SELECTCURRENT.OTHER:
            selected = selector.ByOther(units, near2)
        if selected is None: return [UNIT.CreateIdle(self.mind)]
        return corridor

    def Buffer(self):
        units = [unit for unit in self.mind.access.UNITS_ALL()
                 if self.mind.filters.Valid(unit) and self.mind.filters.Credits(unit, "will")
                 and self.mind.filters.LowCut(unit, "will")]
        units.sort(key=lambda unit: unit.Variable, reverse=True)
        if not units: return [UNIT.CreateIdle(self.mind), self.mind.unit_current]
        index = self.mind.rand.MyRandomInt(1, len(units) - 1)[0]
        index = max(0, min(index, len(units) - 1))
        return [units[index], self.mind.unit_current]

    def Near2(self, unit):
        vector = unit.ToVector2D()
        return vector if self.mind.down.Output(vector) >= 0 else vector.Reverse()

    def Near6(self, unit):
        vector2, vector6 = unit.ToVector2D(), unit.ToVector6D()
        return vector6 if self.mind.down.Output(vector2) >= 0 else vector6.Reverse()
