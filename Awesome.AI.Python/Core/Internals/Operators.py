"""Port of Core/Internals/Operators.cs."""
from awesome_ai.Common.MyNormalize import Norm0DV, Norm1VV
from awesome_ai.CoreSystems.Environment import SimpleAgent
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import HARDDOWN, LOGICTYPE


class Operators:
    def __init__(self, mind):
        self.mind = mind
        self.prop = mind.mech.mp.eprops
        self._ratio = []
        self._errors = []
        self.d_res = 0.0
        self._error = 0

    def Output(self, obj):
        return None

    def Modify(self):
        pass


class Down(Operators):
    def __init__(self, mind):
        super().__init__(mind)
        self.i_decay = 0
        self.abs_max = 0.0

    def Output(self, vec):
        if self.d_res < 0.0:
            return -1
        return 1

    def Modify(self):
        d_curr = self.prop.Conflict()
        d_zero = Norm0DV(d_curr, self.mind)
        d_save = Norm0DV(d_curr, self.mind)
        self.d_res = -1.0 if d_curr < 0.0 else 1.0

        if self.mind.bot.logic == LOGICTYPE.PROBABILITY and self.Probability(d_curr, self.mind):
            d_zero *= -1.0

        if (self.mind.bot.logic == LOGICTYPE.SHARED
                and self.Shared(d_curr, self.mind)
                and self.NoMomentum()):
            d_zero *= -1.0

        flip = d_save != d_zero
        self.d_res = self.d_res * -1.0 if flip else self.d_res
        self.SetError(flip)
        self.SetRatio(self.d_res)

    def Count(self, dir):
        if dir == HARDDOWN.YES:
            return sum(value <= 0.0 for value in self._ratio)
        if dir == HARDDOWN.NO:
            return sum(value > 0.0 for value in self._ratio)
        return 0

    def SetRatio(self, ratio):
        self._ratio.append(ratio)
        if len(self._ratio) > CONST.LAPSES:
            self._ratio.pop(0)

    def SetError(self, err):
        self._errors.append(err)
        if len(self._errors) > 100:
            self._errors.pop(0)
        self._error = sum(value is True for value in self._errors)

    def NoInertia(self):
        # The original returns here, leaving the code below unreachable.
        return True

    def NoMomentum(self):
        mom = self.mind.mech.ms.mom_sym_curr
        absolute = abs(mom)
        if absolute > self.abs_max:
            self.abs_max = absolute
        return absolute < self.abs_max * 0.1

    @staticmethod
    def Shared(_d, mind):
        awareness = 0.0
        agent = SimpleAgent(mind)
        awareA = 1.0 - awareness
        awareB = awareness
        zero = Norm1VV(0.0, mind)
        w_agentA = Norm1VV(_d, mind)
        w_agentB = agent.SimulateDeltaVelocity()
        w_shared = awareA * w_agentA + awareB * w_agentB
        down = w_shared <= zero
        flip = mind.prob.Use(w_shared * 100.0, down, mind)
        agent.SetProperty(flip)
        return flip

    @staticmethod
    def Probability(_d, mind):
        down = _d <= 0
        return mind.prob.Use(_d, down, mind)

