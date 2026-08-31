"""Unit-space construction ported from ``USSetup.cs``."""
import random
from uuid import uuid4
from awesome_ai.Common.MyExtensions import Convert
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import LONGTYPE, MINDS, STATE, TONE, UNITTYPE
from .Unit import UNIT


class USSetup:
    def __init__(self, mind):
        self.mind, self.units_running, self.units_decision, self.learning = mind, [], [], []
        self.location_should_decision = [CONST.LDAT_LOC_SHOULD] * 20
        self.location_what_decision = (
            [CONST.LDAT_LOC_WHAT_u1] * 7
            + [CONST.LDAT_LOC_WHAT_u2] * 7
            + [CONST.LDAT_LOC_WHAT_u3] * 6
        )
        self.answer_should_decision = [CONST.LDAT_ANS_SHOULD] * 8 + [CONST.LDAT_ANS_SHOULD] * 2
        self.answer_what_decision = (
            [CONST.LDAT_ANS_WHAT_u1] * 4
            + [CONST.LDAT_ANS_WHAT_u2] * 4
            + [CONST.LDAT_ANS_WHAT_u3] * 4
        )
        self.ask_should_decision = [CONST.LDAT_ASK_SHOULD] * 5 + [CONST.LDAT_ASK_SHOULD] * 5

        self.Common(CONST.NUMBER_OF_UNITS, self.Tags(mind.mindtype), UNITTYPE.JUSTAUNIT, LONGTYPE.NONE, TONE.RANDOM)

        count = 1
        count = self.Decide(STATE.JUSTRUNNING, 100, CONST.LSUB_SHOULD, self.location_should_decision,
                            UNITTYPE.LDECISION, LONGTYPE.LOCATION, count, TONE.RANDOM)
        count = self.Decide(STATE.JUSTRUNNING, 100, CONST.LSUB_WHAT, self.location_what_decision,
                            UNITTYPE.LDECISION, LONGTYPE.LOCATION, count, TONE.HIGH)
        count = self.Decide(STATE.JUSTRUNNING, 100, CONST.LSUB_SHOULD, self.answer_should_decision,
                            UNITTYPE.LDECISION, LONGTYPE.ANSWER, count, TONE.RANDOM)
        count = self.Decide(STATE.JUSTRUNNING, 100, CONST.LSUB_WHAT, self.answer_what_decision,
                            UNITTYPE.LDECISION, LONGTYPE.ANSWER, count, TONE.LOW)
        self.Decide(STATE.JUSTRUNNING, 100, CONST.LSUB_SHOULD, self.ask_should_decision,
                    UNITTYPE.LDECISION, LONGTYPE.ASK, count, TONE.MID)

    def Tags(self, mindtype):
        if mindtype == MINDS.ANDREW: return list(CONST.sub_andrew)
        if mindtype == MINDS.ROBERTA: return list(CONST.sub_roberta)
        if mindtype == MINDS.BASIC: return list(CONST.sub_basic)
        raise RuntimeError("Memory, Tags")

    def GetIndex(self, tone, value):
        ranges = {TONE.HIGH: (50, 100), TONE.LOW: (0, 50), TONE.MID: (25, 75), TONE.RANDOM: (0, 100)}
        low, high = ranges[tone]
        return Convert(self.mind.calc.Normalize(value, 0.0, 1.0, low, high), self.mind)

    def _random_values(self, tone, count, indexes):
        values = []
        for index in indexes:
            if self.mind.cycles < CONST.FIRST_RUN:
                value = random.random()
            else:
                # The C# code indexes its saved random stream at these same
                # positions (10, 20, ... 60).
                value = self.mind.rand.MyRandomDouble(max(count, index + 1))[index]
            values.append(self.GetIndex(tone, value))
        return values

    def _target(self, state):
        if state == STATE.JUSTRUNNING: return self.units_running
        if state == STATE.QUICKDECISION: return self.units_decision
        raise NotImplementedError("USSetup")

    def Common(self, num_units, subjects, utype, ltype, tone):
        target = self._target(self.mind.STATE)
        for subject in subjects:
            tickets = list(range(1, num_units + 1))
            random.shuffle(tickets)
            guid = str(uuid4())
            for ticket in tickets:
                values = self._random_values(tone, len(subjects), (10, 20, 30, 40, 50, 60))
                target.append(UNIT.Create(self.mind, guid, values, "DATA", f"{subject}{ticket}", utype, ltype))

    def Decide(self, state, index, subject, units, utype, ltype, count, tone):
        target = self._target(state)
        for data in units:
            values = self._random_values(tone, 100, (10, 20, 30, 40, 50, 60))
            target.append(UNIT.Create(self.mind, str(uuid4()), values, data, "NONE", utype, ltype))
            count += 1
        return count
