from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import PATTERN


class MechParams:
    """Mutable mechanics parameter bag, matching ``Core/Mechanics/MechParams.cs``."""

    _fields = """cc_elec_max cc_elec_min dc_elec_max dc_elec_min cc_elec_100 dc_elec_100
    cc_elec_90 dc_elec_90 cc_elec_prev cc_elec_curr dc_elec_prev dc_elec_curr
    vv_100 dv_100 fnet_100 mom_100 acc_100 ke_100
    vv_90 dv_90 fnet_90 mom_90 acc_90 ke_90 dv_prev vv_prev fnet_prev mom_prev
    acc_prev ke_prev dv_curr vv_curr fnet_curr mom_curr acc_curr ke_curr vv_out_high
    vv_out_low dv_out_high dv_out_low fnet_out_high fnet_out_low mom_out_high
    mom_out_low acc_out_high acc_out_low ke_out_high ke_out_low posx_high posx_low
    posxy dt m1 m2 omega eta g damp inertia_lim mu frictionForce batteryVoltage
    variableResistance inductance deltaCurrent f_sta f_dyn f_friction Fmax totalMass
    a F0 beta velocity""".split()

    def __init__(self, **values):
        for name in self._fields:
            setattr(self, name, float(values.get(name, 0.0)))

        # C# declares this as a field initialized from CONST, rather than an
        # auto-property's normal zero value.
        self.pos_x = float(values.get("pos_x", CONST.STARTXY))
        self.mprops = values.get("mprops")
        self.eprops = values.get("eprops")
        self.pattern_curr = values.get("pattern_curr", PATTERN.NONE)
        self.pattern_prev = values.get("pattern_prev", PATTERN.NONE)
