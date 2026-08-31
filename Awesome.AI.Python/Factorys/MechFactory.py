from awesome_ai.Variables.Enums import MECHANICS


class MechFactory:
    def __init__(self, mind=None): self.mind = mind
    def GetMech(self, run, props):
        if run in (MECHANICS.CIRCUIT_1_LOW, MECHANICS.CIRCUIT_2_LOW):
            from awesome_ai.Core.Mechanics.e_CurcuitSimulator import e_CircuitSimulator
            return e_CircuitSimulator(self.mind, run, props)
        from awesome_ai.Core.Mechanics.m_NoiseGenerator import m_NoiseGenerator
        return m_NoiseGenerator(self.mind, run, props)
