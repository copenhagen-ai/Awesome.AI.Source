"""Random Latin word generator ported from ``WordGenerator.cs``."""
from pathlib import Path
import random


class WordGenerator:
    def __init__(self, mind):
        self.mind = mind
        self.path = Path(__file__).resolve().parents[1] / "Data" / "latin.txt"
        self.lines = []
        self.rand = random.Random()

    def LoadData(self):
        if self.lines:
            return
        self.lines = self.path.read_text(encoding="utf-8-sig").splitlines()

    def PickWord(self):
        letters = tuple("ABCDEFGHIJKLMNOPQRSTUVWX")
        letter = self.rand.choice(letters)
        matches = [line for line in self.lines if line and line[0].lower() == letter.lower()]
        if not matches:
            return "..."

        result = self.rand.choice(matches).split(":", 1)[0]
        result = result.split(",", 1)[0]
        result = result.split(" ", 1)[0]
        return "..." if len(result) == 1 else result

    def Format(self, value):
        parenthesized = ""
        copying = False
        for character in value:
            if character == "(":
                copying = True
            if character == ")":
                parenthesized += ")"
                copying = False
            if copying:
                parenthesized += character

        if "(" in value and ")" in value:
            value = value.replace(parenthesized, "")
        return value

    def Clean(self, value):
        allowed = set(" abcdefghijklmnopqrstuvwxyz")
        return "".join(
            character for character in value.strip().lower() if character in allowed
        )

    def Generate(self, idx, sub):
        self.LoadData()
        words = []
        while len(words) < 3:
            word = self.PickWord()
            if word != "...":
                words.append(word)

        result = " ".join(words)
        return self.Clean(self.Format(result))
