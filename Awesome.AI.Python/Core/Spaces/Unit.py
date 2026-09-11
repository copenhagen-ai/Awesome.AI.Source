import random
from dataclasses import dataclass, field
from datetime import datetime, timezone
from uuid import uuid4
from awesome_ai.Common import GPTVector2D, GPTVector6D
from awesome_ai.Common.MyExtensions import HighZero, Index, LowZero
from awesome_ai.Core.Internals.Lookup import Lookup
from awesome_ai.CoreSystems.Environment import Ticket
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import LONGTYPE, MECHANICS, STATE, TRANSFER, UNITTYPE, VALIDATION


@dataclass
class UNIT:
    ticket: object = None
    unit_type: UNITTYPE = UNITTYPE.JUSTAUNIT
    ld_type: LONGTYPE = LONGTYPE.NONE
    created: datetime = field(default_factory=lambda: datetime.now(timezone.utc))
    guid: str = field(default_factory=lambda: str(uuid4()))
    credits: float = 0.0
    reward: float = 0.0
    base_props: dict = field(default_factory=dict)
    h_index: float = 0.0
    data: str = ""
    mind: object = field(default=None, repr=False, compare=False)

    def UIget(self, ax): return 50.0 if self.IsIDLE() else self.base_props[ax]
    def UIset(self, ax, val): self.base_props[ax] = float(val)
    def HIget(self): return 50.0 if self.IsIDLE() else self.h_index
    def HIset(self, val): self.h_index = float(val)

    @staticmethod
    def Create(mind, h_guid, index, data, ticket, ut, lt):
        unit = UNIT(
            ticket=Ticket(ticket or "NOTICKET"), unit_type=ut, ld_type=lt,
            guid=h_guid, data=data, mind=mind, reward=1.0,
            h_index=random.random() * CONST.MAX_HUBSPACE,
        )
        if CONST.USE_CREDITS:
            unit.credits = CONST.MAX_CREDIT
        for axis, value in zip(CONST.AXES, index):
            unit.UIset(axis, value)
        return unit

    @staticmethod
    def CreateIdle(mind): return UNIT.Create(mind, "GUID", [-1.0, -1.0], "IDLE", "NONE", UNITTYPE.IDLE, LONGTYPE.NONE)
    @staticmethod
    def CreateQuick(mind, name, dex): return UNIT.Create(mind, "GUID", dex, name, "NONE", UNITTYPE.QDECISION, LONGTYPE.NONE)
    def ToVector2D(self): return GPTVector2D(self.UIget(CONST.AXES[0]), self.UIget(CONST.AXES[1]))
    def ToVector6D(self): return GPTVector6D(*(self.UIget(k) for k in CONST.AXES))
    def Update2D(self, vector):
        self.UpdateTRA(); self.UpdateREW(); self.UpdateHUB()
        if self.mind.STATE == STATE.QUICKDECISION: return
        if self.Add2D(vector): return
        self.Adjust2D()
    def Update6D(self, vector):
        self.UpdateTRA(); self.UpdateREW(); self.UpdateHUB()
        if self.mind.STATE == STATE.QUICKDECISION: return
        if self.Add6D(vector): return
        self.Adjust6D()
    def UpdateTRA(self):
        if self.IsDECISION(): return
        for unit in self.mind.hub.UnitsPerOccupasionc(): unit.reward = CONST.DECAY * unit.reward + (1.0 if unit.guid == self.guid else 0.0)
    def UpdateREW(self):
        if not self.mind.reward or self.IsDECISION(): return
        units = self.mind.hub.UnitsPerOccupasionc(); maximum = max((unit.reward for unit in units), default=0)
        if maximum: [self.mind.access.UNITS_REM(unit) for unit in list(units) if unit.guid != self.guid and self.mind.calc.Normalize(self.reward, 0, maximum, 0, 1) < CONST.EPSILON1]
    def UpdateHUB(self):
        if self.IsDECISION(): return
        self.mind.reward = False; sub = self.mind.hub.GetSubject(self); index = self.mind.hub.GetIndex(sub)
        if index < 0: return
        units = self.mind.hub.UnitsPerOccupasionc(); maximum = max((unit.reward for unit in units), default=1)
        gamma = CONST.GAMMA * (1 + self.mind.calc.Normalize(self.reward, 0, maximum, 0, 1))
        self.HIset(self.HIget() + (gamma if self.HIget() < index else -gamma)); self.mind.hub.AdjustWeights(sub, gamma * .1)
    def _can_add(self): return len(self.mind.access.UNITS_ALL()) <= CONST.MAX_UNITS * Lookup().CountHUBS(self.mind.mindtype)
    def _axis(self, values): return [[max(CONST.MIN, value - CONST.ALPHA), min(CONST.MAX, value + CONST.ALPHA)] for value in values]
    def Add2D(self, vector):
        if not self._can_add(): return False
        # C# allocates one range per configured axis, while its two first
        # dimensions are populated from the 2D vector.
        axis = self._axis((vector.xx, vector.yy)) + [[CONST.MIN, CONST.MAX] for _ in CONST.AXES[2:]]
        self.mind.access.UNITS_ADD(self, axis, len(CONST.AXES)); return True
    def Add6D(self, vector):
        if not self._can_add(): return False
        self.mind.access.UNITS_ADD(self, self._axis(vector._values()), 6); return True
    def _apply_adjustment(self, values):
        # The C# switch returns after its first matching axis; preserve that
        # behaviour for parity instead of updating every axis at once.
        value = values[0]
        if value < CONST.MIN + CONST.MIN or value > CONST.MAX - CONST.MIN:
            self.mind.access.UNITS_REM(self)
        else:
            self.UIset("will", value)

    def Adjust2D(self):
        random_value = self.mind.rand.MyRandomDouble(10)[5]
        vector, func = self.ToVector2D(), GPTVector2D()
        direction = vector.Unit().ReverseUnit() if self.mind.down.Output(vector) < 0 else vector.Unit()
        updated = func.Add(vector, func.Mul(direction, random_value * CONST.ETA))
        if updated.xx <= CONST.MIN: updated.xx = CONST.MIN
        if updated.yy >= CONST.MAX: updated.yy = CONST.MAX
        self._apply_adjustment((updated.xx, updated.yy))

    def Adjust6D(self):
        random_value = self.mind.rand.MyRandomDouble(10)[5]
        vector2, vector6, func = self.ToVector2D(), self.ToVector6D(), GPTVector6D()
        direction = vector6.Unit().ReverseUnit() if self.mind.down.Output(vector2) < 0 else vector6.Unit()
        updated = func.Add(vector6, func.Mul(direction, random_value * CONST.ETA))
        updated.xx = max(CONST.MIN, updated.xx)
        for name in ("yy", "zz", "ww", "vv", "uu"):
            setattr(updated, name, min(CONST.MAX, getattr(updated, name)))
        self._apply_adjustment(updated._values())
    def IsUNIT(self): return self.unit_type == UNITTYPE.JUSTAUNIT
    def IsIDLE(self): return self.unit_type == UNITTYPE.IDLE
    def IsDECISION(self): return self.unit_type == UNITTYPE.LDECISION
    def IsQDECISION(self): return self.unit_type == UNITTYPE.QDECISION
    @property
    def Data(self):
        if self.data == "QUICK": return "QYES" if self.UIget("will") < 50.0 else "QNO"
        if self.data != "DATA": return self.data
        if self.mind is None: return ""
        subject = self.mind.hub.GetSubject(self)
        if subject == "init": return ""
        return Lookup().GetDATA(self.mind, Index(self.mind.mood.res_norm, self.mind), subject)
    @Data.setter
    def Data(self, value): self.data = value
    @property
    def IsValid(self):
        if self.mind is None: return True
        validation = self.mind.bot.validation
        if validation == VALIDATION.BOTH: return self.mind._internal.Valid(self) and self.mind._external.Valid(self)
        if validation == VALIDATION.EXTERNAL: return self.mind._external.Valid(self)
        if validation == VALIDATION.INTERNAL: return self.mind._internal.Valid(self)
        raise RuntimeError("IsValid")
    @property
    def Root(self):
        units = sorted(self.mind.hub.UnitsPerOccupasionc(), key=lambda unit: unit.created)
        if not units: return ""
        index = units.index(self) + 1 if self in units else 0
        return f"_{self.mind.hub.GetSubject(self)}{index}"
    @property
    def Variable(self):
        if self.mind is None: return 0.0
        mech_type = self.mind.mech.type
        if mech_type == MECHANICS.CIRCUIT_1_LOW: raise RuntimeError("UNIT, Variable")
        if mech_type in (MECHANICS.CIRCUIT_2_LOW, MECHANICS.BALLONHILL_LOW): result = LowZero(self.UIget("will"))
        elif mech_type == MECHANICS.TUGOFWAR_LOW: result = HighZero(self.UIget("will"))
        else: raise RuntimeError("UNIT, Variable")
        if CONST.transfer == TRANSFER.LOGISTIC: return self.mind.calc.Logistic(result * .1 - 5.0) * 100.0
        if CONST.transfer == TRANSFER.OTHER: raise NotImplementedError("UNIT, Variable 1")
        return result
