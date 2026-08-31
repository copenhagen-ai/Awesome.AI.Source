from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import LONGTYPE, ORDER, STATE, UNITTYPE
from .Unit import UNIT


class USAccess:
    def __init__(self, mind): self.mind = mind

    def UNITS_ALL(self, order=ORDER.NONE):
        if self.mind.STATE == STATE.JUSTRUNNING:
            units = self.mind.memory.units_running
            if not units:
                raise RuntimeError("Memory, UNITS_ALL 1")
            if order == ORDER.BYINDEX:
                return sorted(units, key=lambda unit: unit.UIget("will"))
            if order == ORDER.BYVARIABLE:
                return sorted(units, key=lambda unit: unit.Variable)
            return units

        if self.mind.STATE == STATE.QUICKDECISION:
            units = self.mind.memory.units_decision
            if not units:
                raise RuntimeError("Memory, UNITS_ALL 2")
            return list(units)

        raise NotImplementedError("USAccess, UNITS_ALL")

    def UNITS_ADD(self, unit, axis, count):
        values = []
        for index in range(count):
            value = self.mind.rand.MyRandomDouble(1)[0]
            values.append(self.mind.calc.Normalize(value, 0.0, 1.0, axis[index][0], axis[index][1]))

        subjects = self.mind.memory.Tags(self.mind.mindtype)
        random_ticket = self.mind.rand.MyRandomInt(1, len(subjects))[0] + 1
        ticket = f"{self.mind.hub.GetSubject(unit)}{random_ticket}"
        created = UNIT.Create(self.mind, unit.guid, values, "DATA", ticket, UNITTYPE.JUSTAUNIT, LONGTYPE.NONE)
        self.mind.memory.units_running.append(created)

        if self.mind.epochs > CONST.FIRST_RUN * 3:
            self.mind.meters.units_added += 1

    def UNITS_REM(self, unit):
        self.mind.meters.units_removed += 1
        self.mind.memory.units_running.remove(unit)
    def QDRESETU(self): self.mind.memory.units_decision = []
    def QDREMOVE(self, curr): self.mind.memory.units_decision.remove(curr)
    def QDCOUNT(self): return len(self.mind.memory.units_decision)
