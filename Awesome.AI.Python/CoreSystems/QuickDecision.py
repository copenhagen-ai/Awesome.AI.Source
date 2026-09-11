"""Short-lived binary decisions ported from ``QuickDecision.cs``."""
import random

from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import LONGTYPE, STATE, TONE, UNITTYPE


class QuickDecision:
    _RESULT_TYPES = (
        "WHISTLE",
        "MATHLEARN",
        "MATHSOLVE",
        "ARCLEARN",
        "ARCSOLVE",
    )

    def __init__(self, mind):
        self.mind = mind
        self.period = 0
        self.count = {decision_type: 0 for decision_type in self._RESULT_TYPES}
        self.res = {decision_type: False for decision_type in self._RESULT_TYPES}
        self.current = ""

        # Compatibility with the name used by the original Python scaffold.
        self.results = self.res

    def Result(self, decision_type):
        if not self.res[decision_type]:
            return False

        self.count[decision_type] += 1
        if self.count[decision_type] > self.period:
            self.res[decision_type] = False
            self.count[decision_type] = 0

        return self.res[decision_type]

    def Decide(self, curr, decision_type):
        if not curr.IsQDECISION():
            return

        deciding = decision_type in ("QYES", "QNO")
        if not deciding:
            self.Setup(curr, decision_type, 5, 5)
        else:
            self.Run(curr)

    def Run(self, curr):
        if self.mind.STATE == STATE.QUICKDECISION and self.mind.access.QDCOUNT() > 0:
            self.mind.access.QDREMOVE(curr)

        if self.mind.STATE == STATE.QUICKDECISION and self.mind.access.QDCOUNT() == 0:
            self.res[self.current] = curr.Data == "QYES"
            self.mind.STATE = STATE.JUSTRUNNING

    def Setup(self, curr, decision_type, decision_count, period):
        if curr.Data != decision_type or self.mind.STATE != STATE.JUSTRUNNING:
            return

        self.period = period
        self.count[decision_type] = 0

        should_decide = [CONST.QUICK] * decision_count
        should_decide.extend([CONST.QUICK] * decision_count)
        random.shuffle(should_decide)

        self.mind.access.QDRESETU()
        self.mind.memory.Decide(
            STATE.QUICKDECISION,
            100,
            CONST.QSUB_SHOULD,
            should_decide,
            UNITTYPE.QDECISION,
            LONGTYPE.NONE,
            0,
            TONE.RANDOM,
        )

        self.mind.STATE = STATE.QUICKDECISION
        self.current = decision_type
