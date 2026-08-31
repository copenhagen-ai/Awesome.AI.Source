import math

from awesome_ai.Variables.Constants import CONST


class MechHelper:
    def __init__(self, mind=None): self.mind = mind
    def Clamp(self, value, low, high): return max(low, min(high, value))

    def ResetNoise(self, mind, mp):
        if not mind.calc.Chance(CONST.SAMPLE20, 10):
            return
        mp.posxy = CONST.STARTXY
        mp.vv_out_high, mp.vv_out_low = -1000.0, 1000.0
        mp.dv_out_high, mp.dv_out_low = -1000.0, 1000.0
        mp.posx_high, mp.posx_low = -1000.0, 1000.0

    def ResetCircuit(self, mind, mp):
        if not mind.calc.Chance(CONST.SAMPLE20, 10):
            return

        mp.posxy = CONST.STARTXY
        mp.cc_elec_max, mp.cc_elec_min = -1000.0, 1000.0
        mp.dc_elec_max, mp.dc_elec_min = -1000.0, 1000.0
        mp.vv_out_high, mp.vv_out_low = -1000.0, 1000.0
        mp.dv_out_high, mp.dv_out_low = -1000.0, 1000.0
        mp.posx_high, mp.posx_low = -1000.0, 1000.0

    def Reset(self, mp):
        if mp.pattern_prev == mp.pattern_curr:
            return

        mp.pattern_prev = mp.pattern_curr
        mp.pos_x = CONST.STARTXY
        mp.velocity = 0.0
        mp.vv_curr = 0.0
        mp.dv_curr = 0.0
        mp.vv_prev = 0.0
        mp.posx_high, mp.posx_low = -1e10, 1e10

    def NormalizeCircuit(self, mind, mp):
        current_adjustment = 0.1 if mp.cc_elec_min == mp.cc_elec_max else 0.0
        charge_adjustment = 0.1 if mp.dc_elec_min == mp.dc_elec_max else 0.0

        mp.cc_elec_100 = mind.calc.Normalize(
            mp.cc_elec_curr, mp.cc_elec_min - current_adjustment, mp.cc_elec_max, 0.0, 100.0)
        mp.dc_elec_100 = mind.calc.Normalize(
            mp.dc_elec_curr, mp.dc_elec_min - charge_adjustment, mp.dc_elec_max, 0.0, 100.0)
        mp.cc_elec_90 = mind.calc.Normalize(
            mp.cc_elec_curr, mp.cc_elec_min - current_adjustment, mp.cc_elec_max, 10.0, 90.0)
        mp.dc_elec_90 = mind.calc.Normalize(
            mp.dc_elec_curr, mp.dc_elec_min - charge_adjustment, mp.dc_elec_max, 10.0, 90.0)

        values = (mp.cc_elec_100, mp.cc_elec_90, mp.dc_elec_100, mp.dc_elec_90)
        if any(math.isnan(value) for value in values):
            raise RuntimeError("MechHelper, NormalizeCircuit")

    def ExtremesCircuit(self, mp):
        if mp.cc_elec_curr <= mp.cc_elec_min: mp.cc_elec_min = mp.cc_elec_curr
        if mp.cc_elec_curr > mp.cc_elec_max: mp.cc_elec_max = mp.cc_elec_curr
        if mp.dc_elec_curr <= mp.dc_elec_min: mp.dc_elec_min = mp.dc_elec_curr
        if mp.dc_elec_curr > mp.dc_elec_max: mp.dc_elec_max = mp.dc_elec_curr

    def Extremes(self, mp):
        for value, low, high in (
            ("vv_curr", "vv_out_low", "vv_out_high"),
            ("dv_curr", "dv_out_low", "dv_out_high"),
            ("fnet_curr", "fnet_out_low", "fnet_out_high"),
            ("mom_curr", "mom_out_low", "mom_out_high"),
            ("acc_curr", "acc_out_low", "acc_out_high"),
            ("ke_curr", "ke_out_low", "ke_out_high"),
        ):
            current = getattr(mp, value)
            if current <= getattr(mp, low): setattr(mp, low, current)
            if current > getattr(mp, high): setattr(mp, high, current)

    def Normalize(self, mind, mp):
        for name in ("vv", "dv", "fnet", "mom", "acc", "ke"):
            value = getattr(mp, f"{name}_curr")
            low = getattr(mp, f"{name}_out_low")
            high = getattr(mp, f"{name}_out_high")
            adjustment = 0.1 if low == high else 0.0
            setattr(mp, f"{name}_100", mind.calc.Normalize(value, low - adjustment, high, 0.0, 100.0))
            setattr(mp, f"{name}_90", mind.calc.Normalize(value, low - adjustment, high, 10.0, 90.0))

    def PosXY(self, mind, mp):
        x_meter = mp.pos_x
        if x_meter <= 0.1 and not mind.goodbye: x_meter = CONST.VERY_LOW
        x_meter = self.Clamp(x_meter, CONST.LOWXY, CONST.HIGHXY)
        if x_meter <= mp.posx_low: mp.posx_low = x_meter
        if x_meter > mp.posx_high: mp.posx_high = x_meter
        return x_meter

    def Friction(self, mind):
        credits = mind.unit_current.credits
        shift = mind.calc.Normalize(credits, 0.0, 10.0, -5.0, 5.0)
        return mind.calc.Logistic(shift)
