import random
import re


class WordGenerator:
    def __init__(self, mind): self.mind, self.lines, self.rand = mind, [], random.Random()
    def LoadData(self): return self.lines
    def PickWord(self): return self.rand.choice(self.lines) if self.lines else ""
    def Format(self, value): return value.strip().capitalize()
    def Clean(self, value): return re.sub(r"[^\w\s'-]", "", value)
    def Generate(self, idx, sub): return self.Format(self.Clean(f"{idx} {sub}"))

