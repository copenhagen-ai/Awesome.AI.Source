from awesome_ai.Variables.Enums import ACTION, LONGTYPE


class LongDecision:
    def __init__(self, mind, lng_dec=None): self.mind, self.results, self.actions = mind, dict(lng_dec or {}), {}
    def GetResult(self, type): return self.results.get(type, "")
    def Decide(self, _pro, type):
        current = getattr(self.mind, "current", None)
        result = current.data if current else ""
        self.SetResult(type, result, self.mind.cycles)
        return result
    def SetResult(self, type, res, state): self.results[type] = res
    def SetAction(self, mind, type): self.actions[type] = self.GetActionA(mind)
    def GetActionA(self, mind): return ACTION.ACTION if getattr(mind, "current", None) else ACTION.DECLINE
    def GetActionB(self, mind): return self.GetActionA(mind)
