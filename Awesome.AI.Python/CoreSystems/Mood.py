"""Mood generation and temperament validation ported from ``Mood.cs``."""
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import PATTERN, PATTERNCOLOR


class Mood:
    def __init__(self, mind):
        self.mind = mind
        self.Count = 0
        self.res_norm = -1.0
        self.res_color = PATTERNCOLOR.RED
        self.currentmood = PATTERN.NONE

    def Generate(self, _pro):
        if not _pro:
            return

        if self.mind.bot.pattern in (PATTERN.MOODBAD, PATTERN.MOODGENERAL):
            divisor = 5
        else:
            divisor = 10

        if self.Count >= divisor:
            self.Count = 0

        if self.Count == 2:
            random_group = self.mind.rand.MyRandomInt(1, 79)[0] // 10
            if random_group <= 1:
                self.mind.bot.pattern = PATTERN.MOODBAD
            elif random_group <= 3:
                self.mind.bot.pattern = PATTERN.MOODGENERAL
            elif random_group <= 8:
                self.mind.bot.pattern = PATTERN.MOODGOOD
            else:
                raise RuntimeError("MoodGenerator, Generate")

        self.MoodOK(_pro)
        self.Count += 1

    def MoodOK(self, _pro):
        temperament = self.mind.mech.mp.mprops.PropsOut[CONST.prop2_temperament]
        self.res_norm = temperament
        self.currentmood = self.mind.bot.pattern

        if self.currentmood == PATTERN.MOODGENERAL:
            self.res_color = PATTERNCOLOR.GREEN
        elif self.currentmood == PATTERN.MOODGOOD:
            self.res_color = (
                PATTERNCOLOR.GREEN if temperament >= 45.0 else PATTERNCOLOR.RED
            )
        elif self.currentmood == PATTERN.MOODBAD:
            self.res_color = (
                PATTERNCOLOR.GREEN if temperament <= 55.0 else PATTERNCOLOR.RED
            )
