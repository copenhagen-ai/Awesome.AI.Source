from collections import deque
from awesome_ai.Interfaces import IMechanics
from awesome_ai.Variables.Constants import CONST
from .MechParams import MechParams
from .MechSymbolicOut import MechSymbolicOut


class RCStage:
    def __init__(self, resistance=1.0, capacitance=1.0): self.resistance, self.capacitance, self.output = resistance, capacitance, 0.0
    def Step(self, inputVoltage, gain1, gain2, dt):
        tau = max(self.resistance * self.capacitance, 1e-12)
        self.output += (inputVoltage * gain1 * gain2 - self.output) * min(dt / tau, 1.0)
        return self.output


class FeedbackRegister:
    def __init__(self, size=8): self.values = deque(maxlen=size)
    def Average(self): return sum(self.values) / len(self.values) if self.values else 0.0
    def Push(self, value): self.values.append(float(value))


class e_CircuitSimulator(IMechanics):
    def __init__(self, mind=None, type=None, props=None):
        self.mind, self.type, self.props = mind, type, props
        self.mp, self.ms = MechParams(), MechSymbolicOut()
        self.stage = RCStage(); self.feedback = FeedbackRegister()
    def PosXY(self): return self.mp.posxy
    def Calc(self, curr, cycles):
        for _ in range(cycles):
            self.mp.posxy = self.stage.Step(self.mp.batteryVoltage, 1.0, 1.0, self.mp.dt or .01)
            self.feedback.Push(self.mp.posxy)
    def DeltaTime(self): return self.mp.dt
    def Damping(self, mind):
        if not CONST.USE_CREDITS:
            return -1.0
        return self.feedback.Average()
    def Calculate(self, match, cycles): self.Calc(None, cycles); self.ms.Convert(self.mp, self.type)
