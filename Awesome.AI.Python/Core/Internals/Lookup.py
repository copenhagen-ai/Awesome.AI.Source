"""Subject lookup and semantic hub ordering from ``Lookup.cs``."""
from dataclasses import dataclass

from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import MINDS
from .LookupData import DATA


@dataclass(frozen=True)
class Entry:
    Name: str
    Social: int
    Hobby: int


class Lookup:
    """Maps every mind type to its C# subjects and semantic occupations."""

    roberta_sem = (
        Entry("love", 6, -1), Entry("macho machines", 5, -1),
        Entry("music", 4, -1), Entry("friends", 3, -1),
        Entry("socializing", 2, -1), Entry("dancing", 1, 5),
        Entry("movies", -1, 4), Entry("hobbys", -1, 3),
        Entry("the weather", -1, 2), Entry("having fun", -1, 1),
    )
    andrew_sem = (
        Entry("procrastination", 6, -1), Entry("fembots", 5, -1),
        Entry("power tools", 4, -1), Entry("cars", 3, -1),
        Entry("movies", 2, -1), Entry("programming", 1, 5),
        Entry("the weather", -1, 4), Entry("life", -1, 3),
        Entry("computers", -1, 2), Entry("work", -1, 1),
    )
    basic_sem = (
        Entry("love", 5, -1), Entry("macho machines", 6, -1),
        Entry("music", 3, -1), Entry("friends", 4, -1),
        Entry("socializing", 2, -1), Entry("dancing", 1, 4),
        Entry("movies", -1, 5), Entry("hobbys", -1, 2),
        Entry("the weather", -1, 3), Entry("having fun", -1, 1),
    )
    SUBJECTS = {
        MINDS.ROBERTA: CONST.sub_roberta,
        MINDS.ANDREW: CONST.sub_andrew,
        MINDS.BASIC: CONST.sub_basic,
    }

    def _entries(self, mindtype):
        if mindtype == MINDS.ROBERTA:
            return self.roberta_sem
        if mindtype == MINDS.ANDREW:
            return self.andrew_sem
        if mindtype == MINDS.BASIC:
            return self.basic_sem
        raise ValueError(f"Lookup, unknown mind type: {mindtype}")

    def GetDATA(self, mind, idx, sub):
        """Return the C# contextual sentence for a subject/index pair."""
        subject = str(sub).strip().lower()
        if subject not in self.SUBJECTS[mind.mindtype]:
            raise ValueError(f"Lookup, unknown subject: {subject}")
        mind_data = DATA[mind.mindtype.name.lower()]
        try:
            text = mind_data[subject][str(idx)]
        except KeyError as error:
            raise ValueError(f"Lookup, missing data for {subject} [{idx}]") from error
        text = text.strip().lower().replace(".", "").replace("?", "")
        return f"{text} [{idx}]"

    def GetHUBS(self, mindtype, axis_or_count=None, with_occupation=False):
        """Return hubs ordered by occupation, with a Python out-value option."""
        if axis_or_count is None:
            return list(self.SUBJECTS[mindtype])
        if isinstance(axis_or_count, int):
            if axis_or_count < 0 or axis_or_count >= len(CONST.occupasions):
                raise ValueError("Lookup, GetHUBS")
            occupation = CONST.occupasions[axis_or_count]
            hubs = self.GetHUBS(mindtype, occupation)
            return (hubs, occupation) if with_occupation else hubs

        occupation = axis_or_count
        if occupation == "init":
            return []
        if occupation not in CONST.occupasions:
            return list(self.SUBJECTS[mindtype])
        social = occupation == CONST.occupasions[0]
        entries = sorted(
            self._entries(mindtype),
            key=lambda entry: entry.Social if social else entry.Hobby,
            reverse=True,
        )
        return [entry.Name for entry in entries if (entry.Social if social else entry.Hobby) > 0]

    def CountHUBS(self, mindtype):
        return len(self._entries(mindtype))
