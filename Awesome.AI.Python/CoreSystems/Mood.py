from awesome_ai.Variables.Enums import PATTERN, PATTERNCOLOR


class Mood:
    def __init__(self, mind):
        self.mind, self.Count = mind, 0
        self.res_norm, self.res_color, self.currentmood = -1.0, PATTERNCOLOR.RED, PATTERN.NONE
    def Generate(self, _pro):
        units = self.mind.memory.units_running
        self.res_norm = sum(u.reward for u in units) / len(units) if units else 0.0
        self.res_color = PATTERNCOLOR.GREEN if self.res_norm >= 0 else PATTERNCOLOR.RED
        self.Count += 1; return self.res_norm
    def MoodOK(self, _pro): return self.res_color == PATTERNCOLOR.GREEN

