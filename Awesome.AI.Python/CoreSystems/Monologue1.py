class Monologue1:
    def __init__(self, mind): self.mind, self.Result, self.Subject, self.Relevance = mind, "", "", ""
    def GetRelevance(self, str1, str2): return "high" if str1 == str2 else "low"
    def Create(self, _pro):
        unit = getattr(self.mind, "current", None); self.Subject = unit.data if unit else ""
        self.Result = self.Subject; self.Relevance = self.GetRelevance(self.Result, self.Subject)
        return self.Result

