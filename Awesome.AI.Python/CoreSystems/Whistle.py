"""Whistle output behavior ported from ``Whistle.cs``."""


class Whistle:
    def __init__(self, mind):
        self.mind = mind
        self.mind.result_whistle = ""
        self.gimmick = ("[.??]", "[??.]")
        self.count = 0
        self.num = 0

    def Do(self, _pro):
        if not _pro:
            return

        if self.count > 1:
            self.count = 0

        result = self.mind._quick.Result("WHISTLE")
        if result:
            self.num += 1
            self.mind.result_whistle = f"[{self.num}][Whistling to my self..]"
        else:
            self.mind.result_whistle = f"[{self.num}]" + self.gimmick[self.count]

        self.count += 1
