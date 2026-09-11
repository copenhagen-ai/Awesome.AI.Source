"""Deterministic inner monologue ported from ``Monologue1.cs``."""
from awesome_ai.Variables.Enums import STATE


class Monologue1:
    def __init__(self, mind):
        self.mind = mind
        self.Result = ""
        self.Subject = ""
        self.Relevance = ""
        self.prev = "im so happy"
        self.curr = ""
        self.counter = 0

    def GetRelevance(self, str1, str2):
        number1 = "".join(character for character in str1 if character.isdigit())
        number2 = "".join(character for character in str2 if character.isdigit())

        if str1 == "im so happy":
            return ", but "

        upper1 = int(number1) > 4
        upper2 = int(number2) > 4
        if upper1 == upper2:
            return ", and "
        return ", but "

    def Create(self, _pro):
        if not _pro:
            return

        self.counter += 1
        if self.counter < 2:
            return
        self.counter = 0

        if self.mind.STATE == STATE.QUICKDECISION:
            return

        unit = self.mind.unit_actual
        if unit.IsQDECISION() or unit.IsDECISION():
            return

        subject = self.mind.hub.GetSubject(unit) or ""
        self.curr = unit.Data if unit is not None else ""

        if subject in ("", "init"):
            return

        self.Relevance = self.GetRelevance(self.prev, self.curr)
        result = self.prev + "||" + self.curr
        self.prev = self.curr
        self.Result = result
        self.Subject = subject
