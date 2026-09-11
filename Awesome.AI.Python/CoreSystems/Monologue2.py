"""Latent/generated inner monologue ported from ``Monologue2.cs``."""
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import STATE


class Monologue2:
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

    def Index(self, value):
        normalized = self.mind.calc.Normalize(value, 10.0, 90.0, 0.0, 100.0)
        if normalized < 10.0:
            return "0"
        return str(normalized)[0]

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
        index = self.Index(self.mind.mood.res_norm)

        if subject in ("", "init") or CONST.DECI_SUBJECT_CONTAINS(subject):
            return

        current = self.mind.word.Generate(index, subject)
        current = current.strip().lower().replace(".", "").replace("?", "")
        for number in range(9, -1, -1):
            current = current.replace(f"[{number}]", "")
        self.curr = f" [{subject}, {index}] " + current

        self.Relevance = self.GetRelevance(self.prev, self.curr)
        result = self.prev + "||" + self.curr
        self.prev = self.curr
        self.Result = result
        self.Subject = subject
