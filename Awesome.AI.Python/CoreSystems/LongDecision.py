"""Multi-step decisions ported from ``LongDecision.cs``."""
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import ACTION, LONGTYPE, MINDS


class LongDecision:
    def __init__(self, mind, decisions=None):
        self.mind = mind
        self.Result = {}
        self.State = {}
        self.index = []

        for decision_type, result in (decisions or {}).items():
            self.State[decision_type] = 0
            self.Result[decision_type] = result.replace("WHAT", "")

        # Compatibility with the name used by the original Python scaffold.
        self.results = self.Result

    def GetResult(self, decision_type):
        result = self.Result[decision_type]
        self.Result[decision_type] = ""
        return result

    def Decide(self, _pro, decision_type):
        if not _pro or self.mind.epochs < 5:
            return

        self.SetAction(self.mind, decision_type)

        unit = self.mind.unit_current
        if not unit.IsDECISION() or unit.ld_type != decision_type:
            return
        if decision_type == LONGTYPE.ASK and self.mind.chat_asked:
            return
        if decision_type == LONGTYPE.ASK and self.State[LONGTYPE.ANSWER] > 0:
            return

        self.GetActionA(self.mind)
        action_b = self.GetActionB(self.mind)
        subject = self.mind.hub.GetSubject(unit) or ""

        if subject == CONST.LSUB_SHOULD and self.State[decision_type] == 0:
            if unit.Data == CONST.LDAT_LOC_SHOULD and action_b == ACTION.ACTION:
                self.SetResult(decision_type, "", 1)

            if unit.Data == CONST.LDAT_ANS_SHOULD:
                if action_b == ACTION.ACTION:
                    self.SetResult(decision_type, ":YES", 0)
                elif action_b == ACTION.DECLINE:
                    self.SetResult(decision_type, "Im busy right now..", 0)

            if unit.Data == CONST.LDAT_ASK_SHOULD and action_b == ACTION.ACTION:
                self.SetResult(decision_type, self.mind.hub.GetSubject(unit), 0)

        if subject == CONST.LSUB_WHAT and self.State[decision_type] == 1:
            new_result = unit.Data.replace(CONST.lng_what, "")
            if self.mind.down.d_res < 0.0:
                self.SetResult(decision_type, "", 0)
            elif unit.Data != CONST.lng_what + self.Result[decision_type]:
                self.SetResult(decision_type, new_result, 0)

    def SetResult(self, decision_type, result, state):
        if state == 1:
            self.mind.reward = True
        if result != "":
            self.Result[decision_type] = result
        self.State[decision_type] = state

    @property
    def thres(self):
        if self.mind.mindtype == MINDS.ROBERTA:
            return 23.0
        if self.mind.mindtype in (MINDS.ANDREW, MINDS.BASIC):
            return 40.0
        raise RuntimeError("LongDecision threshold")

    def SetAction(self, mind, decision_type):
        if decision_type != LONGTYPE.LOCATION:
            return
        self.index.append(mind.unit_current.UIget("will"))
        if len(self.index) > 10:
            self.index.pop(0)

    def GetActionA(self, mind):
        average = sum(self.index) / len(self.index)
        return ACTION.ACTION if average > self.thres else ACTION.DECLINE

    def GetActionB(self, mind):
        average = sum(self.index) / len(self.index)
        current = mind.unit_current.UIget("will")
        return ACTION.ACTION if current >= average else ACTION.DECLINE
