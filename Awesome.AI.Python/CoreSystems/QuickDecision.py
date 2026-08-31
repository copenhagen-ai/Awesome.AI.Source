class QuickDecision:
    def __init__(self, mind): self.mind, self.results, self.period = mind, {}, 0
    def Result(self, type): return self.results.get(type, False)
    def Decide(self, curr, type): self.results[type] = bool(curr and curr.credits >= 0); return self.results[type]
    def Run(self, curr): return self.Decide(curr, "default")
    def Setup(self, curr, type, _c, per): self.period = per; return self.Decide(curr, type)

