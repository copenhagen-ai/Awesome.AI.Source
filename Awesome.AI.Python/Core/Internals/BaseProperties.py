class BaseProperties(dict):
    """Dictionary-backed equivalent of the C# base property collection."""

    def Get(self, key, default=0.0): return self.get(key, default)
    def Set(self, key, value): self[key] = float(value)
    def __init__(self, ms=None): super().__init__(); self.ms = ms
    def Will(self): return self.ms.vv_sym_curr if self.ms else 0.0
    def Conflict(self): return self.ms.dv_sym_curr if self.ms else 0.0
    def Commitment(self): return self.ms.mom_sym_curr if self.ms else 0.0
    def Adaptation(self): return self.ms.acc_sym_curr if self.ms else 0.0
    def Activation(self): return self.ms.ke_sym_curr if self.ms else 0.0
    def Influence(self): return self.ms.fnet_sym_curr if self.ms else 0.0
    def Creativity(self):
        if not self.ms or self.ms.vv_sym_curr == 0 or self.ms.vv_sym_prev == 0: return 0
        return abs(self.ms.vv_sym_curr - self.ms.vv_sym_prev)
