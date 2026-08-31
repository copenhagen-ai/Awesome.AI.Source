from awesome_ai.Common.MyNormalize import Norm100VV
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import PATTERN, PROPS


class ModProperties:
    class MyModifiers:
        def Mod(self, mind, value, prop):
            if prop == "base": return value
            if prop == CONST.prop2_temperament:
                pattern = mind.mech.mp.pattern_curr
                if pattern == PATTERN.MOODGENERAL: return mind.calc.Normalize(value, 0, 100, 10, 90)
                if pattern == PATTERN.MOODGOOD: return mind.calc.Normalize(value, 0, 100, 60, 90)
                if pattern == PATTERN.MOODBAD: return mind.calc.Normalize(value, 0, 100, 10, 40)
                return 0.0
            messages = {CONST.prop2_brain: "ModProperties, Mod 1", CONST.prop3_brain: "ModProperties, Mod 2",
                        CONST.prop2_comm: "ModProperties, Mod 3", CONST.prop3_comm: "ModProperties, Mod 4",
                        CONST.prop4_comm: "ModProperties, Mod 5"}
            raise RuntimeError(messages.get(prop, "ModProperties, Mod 6"))

    class MyMatrix:
        def __init__(self): self._data = {}
        def __getitem__(self, keys): return self._data.get(tuple(keys), 1.0)
        def __setitem__(self, keys, value): self._data[tuple(keys)] = value
        def Run(self, val, key2):
            result = 1.0
            for key1, _ in self._data: result *= self[key1, key2]
            return result * val

    def __init__(self, mind, props):
        self.mind = mind
        attr, matrix = self.GetProps(props)
        self.PropsOut = {key: 0.0 for key in attr}
        self.PropsIn = attr
        self.Mods, self.Matrix = self.MyModifiers(), self.MyMatrix()
        for keys, value in matrix.items(): self.Matrix[keys] = value

    def Update(self):
        normalized = self.GetBase(self.mind.bot.props)
        for prop in self.PropsIn: self.PropsOut[prop] = self.Mods.Mod(self.mind, normalized, prop)

    def GetProps(self, props):
        if props == PROPS.TEMPERAMENT:
            return ({"base": float("nan"), CONST.prop2_temperament: 2.0}, {("base", CONST.prop1_temperament): 1.0})
        if props == PROPS.BRAINWAVE:
            return ({"base": float("nan"), CONST.prop2_brain: 2.0, CONST.prop3_brain: 1.2},
                    {("base", CONST.prop3_brain): 1.0, (CONST.prop2_brain, CONST.prop3_brain): 1.0})
        if props == PROPS.COMMUNICATION:
            return ({"base": float("nan"), CONST.prop2_comm: 2.0, CONST.prop3_comm: .5, CONST.prop4_comm: 1.2},
                    {("base", CONST.prop3_comm): .65, (CONST.prop2_comm, CONST.prop3_comm): .45,
                     (CONST.prop4_comm, CONST.prop2_comm): .35})
        raise NotImplementedError("Properties, GetProps")

    def GetBase(self, props):
        if props in (PROPS.TEMPERAMENT, PROPS.BRAINWAVE, PROPS.COMMUNICATION):
            return Norm100VV(self.mind.mech.mp.eprops.Will(), self.mind)
        raise NotImplementedError("Properties, GetProps")
