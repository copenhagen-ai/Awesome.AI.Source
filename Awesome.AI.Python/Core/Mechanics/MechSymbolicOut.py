from awesome_ai.Variables.Enums import MECHANICS


class MechSymbolicOut:
    """Mechanics values exposed through the common symbolic output shape."""
    _fields = """vv_sym_peek vv_sym_low_peek vv_sym_high_peek dv_sym_prev vv_sym_prev
    fnet_sym_prev mom_sym_prev acc_sym_prev ke_sym_prev dv_sym_curr vv_sym_curr
    fnet_sym_curr mom_sym_curr acc_sym_curr ke_sym_curr m1_sym m2_sym fsta_sym fdyn_sym
    vv_sym_low vv_sym_high dv_sym_low dv_sym_high fnet_sym_low fnet_sym_high mom_sym_low
    mom_sym_high acc_sym_low acc_sym_high ke_sym_low ke_sym_high vv_sym_100 dv_sym_100
    fnet_sym_100 mom_sym_100 acc_sym_100 ke_sym_100 vv_sym_90 dv_sym_90 fnet_sym_90
    mom_sym_90 acc_sym_90 ke_sym_90 peek_sym_norm""".split()

    def __init__(self):
        for name in self._fields: setattr(self, name, 0.0)

    def Convert(self, mp, type):
        if type in (MECHANICS.TUGOFWAR_LOW, MECHANICS.BALLONHILL_LOW):
            self.dv_sym_prev = mp.dv_prev
            self.vv_sym_prev = mp.vv_prev
            self.fnet_sym_prev = mp.fnet_prev
            self.mom_sym_prev = mp.mom_prev
            self.acc_sym_prev = mp.acc_prev
            self.ke_sym_prev = mp.ke_prev

            self.vv_sym_curr = mp.vv_curr
            # These assignments intentionally follow the C# mapping: all of
            # the remaining current symbolic values use delta velocity.
            self.dv_sym_curr = mp.dv_curr
            self.mom_sym_curr = mp.dv_curr
            self.acc_sym_curr = mp.dv_curr
            self.ke_sym_curr = mp.dv_curr
            self.fnet_sym_curr = mp.dv_curr

            self.vv_sym_low, self.vv_sym_high = mp.vv_out_low, mp.vv_out_high
            self.dv_sym_low, self.dv_sym_high = mp.dv_out_low, mp.dv_out_high
            self.fnet_sym_low, self.fnet_sym_high = mp.fnet_out_low, mp.fnet_out_high
            self.mom_sym_low, self.mom_sym_high = mp.mom_out_low, mp.mom_out_high
            self.acc_sym_low, self.acc_sym_high = mp.acc_out_low, mp.acc_out_high
            self.ke_sym_low, self.ke_sym_high = mp.ke_out_low, mp.ke_out_high

            self.vv_sym_100, self.dv_sym_100 = mp.vv_100, mp.dv_100
            self.fnet_sym_100, self.mom_sym_100 = mp.fnet_100, mp.mom_100
            self.acc_sym_100, self.ke_sym_100 = mp.acc_100, mp.ke_100
            self.vv_sym_90, self.dv_sym_90 = mp.vv_90, mp.dv_90
            self.fnet_sym_90, self.mom_sym_90 = mp.fnet_90, mp.mom_90
            self.acc_sym_90, self.ke_sym_90 = mp.acc_90, mp.ke_90

            self.m1_sym, self.m2_sym = mp.m1, mp.m2
            self.fsta_sym, self.fdyn_sym = mp.f_sta, mp.f_dyn
        elif type in (MECHANICS.CIRCUIT_1_LOW, MECHANICS.CIRCUIT_2_LOW):
            self.vv_sym_curr, self.dv_sym_curr = mp.cc_elec_curr, mp.dc_elec_curr
            self.vv_sym_low, self.vv_sym_high = mp.cc_elec_min, mp.cc_elec_max
            self.dv_sym_low, self.dv_sym_high = mp.dc_elec_min, mp.dc_elec_max
            self.vv_sym_100, self.dv_sym_100 = mp.cc_elec_100, mp.dc_elec_100
            self.vv_sym_90, self.dv_sym_90 = mp.cc_elec_90, mp.dc_elec_90
