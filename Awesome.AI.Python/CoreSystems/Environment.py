"""Internal/external environment maps ported from ``Environment.cs``."""
from dataclasses import dataclass, field

from awesome_ai.Core.Internals.Lookup import Lookup
from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import MINDS, OCCUPASION, STATE, TAGS, VALIDATION


class SimpleAgent:
    def __init__(self, mind): self.mind, self.Property = mind, False
    def SimulateDeltaVelocity(self): return self.mind.rand.MyRandomDouble(1)[0]
    def SimulateDirection(self): return -1.0 if self.mind.rand.MyRandomDouble(1)[0] <= 0.5 else 1.0
    def SimulateYes(self): return self.SimulateDirection() <= 0.0
    def SetProperty(self, _b): self.Property = _b


# Backward-compatible name used by the first scaffold.
Environment = SimpleAgent


class TimeLine:
    """Reserved C# timeline model for later memory/event expansion."""


@dataclass
class Occupasion:
    name: str = ""
    max_epochs: int = 0
    values: list[str] = field(default_factory=list)


@dataclass
class Ticket:
    t_name: str = ""


class MyInternal:
    """MapMind: selects the current internal occupation and its valid hubs."""

    def __init__(self, mind=None):
        self.mind = mind
        self.occus = []
        self.occu = Occupasion(name="init", max_epochs=10, values=[])
        self.run = False
        self.epoch_old = -1
        self.epoch_count = 0
        self.epoch_stop = -1

    @property
    def Occu(self):
        self.run = self.mind.epochs != self.epoch_old
        self.epoch_old = self.mind.epochs
        if not self.run:
            return self.occu

        if self.mind.bot.occupasion == OCCUPASION.FIXED:
            hubs, occupation = Lookup().GetHUBS(self.mind.mindtype, 0, with_occupation=True)
            self.occu = Occupasion(name=occupation, max_epochs=30, values=hubs)
        elif self.mind.bot.occupasion == OCCUPASION.DYNAMIC:
            if self.epoch_count > self.epoch_stop:
                self.epoch_count = 0
                maximum = max(1, self.occu.max_epochs)
                self.epoch_stop = self.mind.rand.Next(1, maximum + 1)
                if self.occus:
                    self.occu = self.occus[self.mind.rand.Next(len(self.occus))]
        else:
            raise RuntimeError("Occu")

        self.epoch_count += 1
        return self.occu

    def Valid(self, unit):
        if unit is None:
            return False
        if unit.IsDECISION():
            return True
        try:
            occupation = self.Occu
            subject = self.mind.hub.GetSubject(unit) or ""
            if not occupation.name or not subject:
                return False
            configured = next(item for item in self.occus if item.name == occupation.name)
            return subject in configured.values
        except (StopIteration, AttributeError, TypeError):
            return False

    def Setup(self):
        self.occus = []
        lookup = Lookup()
        mindtypes = (MINDS.BASIC,) if self.mind.mindtype == MINDS.BASIC else (MINDS.ROBERTA, MINDS.ANDREW)
        for mindtype in mindtypes:
            for index in range(len(CONST.occupasions)):
                hubs, occupation = lookup.GetHUBS(mindtype, index, with_occupation=True)
                self.occus.append(Occupasion(name=occupation, max_epochs=30, values=hubs))

    def Reset(self):
        if self.mind.STATE != STATE.QUICKDECISION and self.mind.bot.validation != VALIDATION.EXTERNAL:
            self.Setup()
            # HubSpace consults Occu during Python construction; restart the
            # epoch gate so the populated map is selected on the next access.
            self.epoch_old = -1
            self.epoch_count = 0
            self.epoch_stop = -1


class MyExternal:
    @dataclass
    class Tag:
        t_name: str = ""
    def __init__(self, mind=None):
        self.mind = mind
        self.tags = []

    @staticmethod
    def _ticket_name(unit):
        ticket = unit.ticket
        return ticket.t_name if hasattr(ticket, "t_name") else str(ticket or "")

    def Valid(self, unit):
        if unit is None:
            return False
        if unit.IsDECISION() or unit.IsQDECISION():
            return True
        ticket_name = self._ticket_name(unit)
        if not ticket_name:
            raise RuntimeError("Valid")
        self.tags = [tag for tag in self.tags if tag is not None]
        return any(tag.t_name == ticket_name for tag in self.tags)

    def Setup(self, mindtype, onlyeven):
        self.tags = [self.Tag("SPECIAL")]
        for subject in self.mind.memory.Tags(mindtype):
            for number in range(1, CONST.NUMBER_OF_UNITS + 1):
                if onlyeven and number % 2 == 0:
                    continue
                self.tags.append(self.Tag(f"{subject}{number}"))

    def Reset(self):
        if self.mind.bot.validation == VALIDATION.INTERNAL:
            return
        if self.mind.bot.tags == TAGS.ALL:
            self.Setup(self.mind.mindtype, False)
        elif self.mind.bot.tags == TAGS.EVEN:
            self.Setup(self.mind.mindtype, True)
        else:
            raise RuntimeError("Reset")
