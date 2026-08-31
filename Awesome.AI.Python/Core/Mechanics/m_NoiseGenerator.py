"""Force-based port of ``Core/Mechanics/m_NoiseGenerator.cs``."""
from __future__ import annotations

import math

from awesome_ai.Interfaces import IMechanics
from awesome_ai.Core.Internals.BaseProperties import BaseProperties
from awesome_ai.Core.Internals.ModProperties import ModProperties
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import MECHANICS

from .MechHelper import MechHelper
from .MechParams import MechParams
from .MechSymbolicOut import MechSymbolicOut


def _sign(value: float) -> float:
    return float((value > 0) - (value < 0))


class m_NoiseGenerator(IMechanics):
    def __init__(self, mind=None, type=None, props=None):
        self.mind, self.type = mind, type
        self.ms = MechSymbolicOut()
        self.mh = MechHelper()
        self.mp = MechParams()
        self.mp.mprops = ModProperties(mind, props)
        self.mp.eprops = BaseProperties(self.ms)
        self.mp.posxy = CONST.STARTXY
        self.mp.vv_out_high, self.mp.vv_out_low = -1000.0, 1000.0
        self.mp.dv_out_high, self.mp.dv_out_low = -1000.0, 1000.0
        self.mp.posx_high, self.mp.posx_low = -1000.0, 1000.0

    def PosXY(self):
        return self.mh.PosXY(self.mind, self.mp)

    def Calc(self, curr, cycles):
        if self.type == MECHANICS.TUGOFWAR_LOW:
            self.mp.damp, self.mp.inertia_lim = 1.0, 0.15
            self.mp.m1, self.mp.m2, self.mp.dt, self.mp.Fmax = 1500.0, 1500.0, 1.0, 3000.0
        elif self.type == MECHANICS.BALLONHILL_LOW:
            self.mp.damp, self.mp.inertia_lim = 1.0, 0.15
            self.mp.a, self.mp.g, self.mp.m1, self.mp.dt = 0.1, CONST.GRAVITY, 0.35, 1.0
        else:
            raise RuntimeError("m_NoiseGenerator, Calc")

        self.mp.f_sta = self.ApplyStatic(self.mp, self.type)
        self.mp.f_dyn = self.ApplyDynamic(self.mp, self.type, curr)
        self.mp.f_friction = self.Friction3(self.mp, self.type)
        force = self.mp.f_sta + self.mp.f_dyn + self.mp.f_friction

        for name in ("dv", "vv", "mom", "acc", "ke", "fnet"):
            setattr(self.mp, f"{name}_prev", getattr(self.mp, f"{name}_curr"))

        total_mass = self.mp.m1 + self.mp.m2
        self.mp.dv_curr = (force * self.mp.dt) / total_mass
        self.mp.vv_curr = self.mp.vv_prev + self.mp.dv_curr
        self.mp.mom_curr = self.mp.vv_curr * total_mass
        self.mp.acc_curr = self.mp.dv_curr / self.mp.dt
        self.mp.ke_curr = 0.5 * total_mass * self.mp.vv_curr ** 2
        self.mp.fnet_curr = force
        self.mp.pos_x += self.mp.vv_curr * self.mp.dt

        # The source implementation rejects NaN here.  Infinity is left to
        # propagate just as it would in the C# mechanics calculation.
        if any(math.isnan(value) for value in (self.mp.dv_prev, self.mp.dv_curr, self.mp.vv_prev, self.mp.vv_curr)):
            raise RuntimeError("NAN")

    def Logic(self, curr):
        if self.type == MECHANICS.TUGOFWAR_LOW:
            v0 = 0.01 * curr.Variable
            v1 = math.tanh(1.8 * v0)
            v2 = 0.8 * v0
            return v1 if self.mp.vv_curr < 0.0 else v2
        if self.type == MECHANICS.BALLONHILL_LOW:
            return self.mind.unit_current.Variable * 0.2
        raise RuntimeError("m_NoiseGenerator, Friction")

    def Friction1(self, mp, type):
        if self.mind.goodbye: return 0.0
        if type not in (MECHANICS.TUGOFWAR_LOW, MECHANICS.BALLONHILL_LOW):
            raise RuntimeError("m_NoiseGenerator, Friction")
        return self.mh.Friction(self.mind) * 0.001 * mp.m1 * CONST.GRAVITY * -_sign(mp.vv_curr)

    def Friction2(self, mp, type):
        if self.mind.goodbye: return 0.0
        if type not in (MECHANICS.TUGOFWAR_LOW, MECHANICS.BALLONHILL_LOW):
            raise RuntimeError("m_NoiseGenerator, Friction")
        return -(0.1 * mp.vv_curr)

    def Friction3(self, mp, type):
        if self.mind.goodbye: return 0.0
        return 500.0 * self.mh.Friction(self.mind) * -_sign(mp.vv_curr)

    def Friction4(self, mp, type):
        if self.mind.goodbye: return 0.0
        return 500.0 * self.mh.Friction(self.mind) * -_sign(mp.f_sta + mp.f_dyn)

    def ApplyStatic(self, mp, type):
        if mp.acc_curr == 0.0: mp.acc_curr = 1.0
        if type == MECHANICS.TUGOFWAR_LOW:
            return -(mp.damp * CONST.BASE_SCALE * mp.Fmax)
        if type == MECHANICS.BALLONHILL_LOW:
            slope = 2 * mp.a * mp.pos_x
            sin_theta = slope / math.sqrt(1 + slope * slope)
            return -(mp.damp * (mp.m1 * mp.g) * sin_theta)
        raise RuntimeError("m_NoiseGenerator, ApplyStatic")

    def ApplyDynamic(self, mp, type, curr):
        if self.mind.goodbye: return 0.0
        if mp.acc_curr == 0.0: mp.acc_curr = 1.0
        if type == MECHANICS.TUGOFWAR_LOW:
            return mp.damp * self.Logic(curr) * mp.Fmax
        if type == MECHANICS.BALLONHILL_LOW:
            return mp.damp * mp.m1 * self.Logic(curr)
        raise RuntimeError("m_NoiseGenerator, ApplyDynamic")

    def Calculate(self, match, cycles):
        pattern = self.mind.bot.pattern
        if pattern != match: return
        self.mp.pattern_curr = pattern
        if cycles == 1: self.mh.ResetNoise(self.mind, self.mp)
        self.Calc(self.mind.unit_current, cycles)
        self.mh.Extremes(self.mp)
        self.mh.Normalize(self.mind, self.mp)
        self.ms.Convert(self.mp, self.type)
