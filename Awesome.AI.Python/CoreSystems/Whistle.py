class Whistle:
    def __init__(self, mind): self.mind, self.count = mind, 0
    def Do(self, _pro): self.count += 1; return self.count

