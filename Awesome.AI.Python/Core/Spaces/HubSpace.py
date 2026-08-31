"""Weighted hub-space port of ``Core/Spaces/HubSpace.cs``."""
import random

from awesome_ai.Core.Internals.Lookup import Lookup
from awesome_ai.Variables.Constants import CONST


class HubSpace:
    def __init__(self, mind):
        self.mind, self.weights = mind, {}
        lookup = Lookup()
        for occupation in CONST.occupasions:
            for hub in lookup.GetHUBS(mind.mindtype, occupation):
                if hub not in self.weights:
                    self.weights[hub] = random.random()

    def _occupation(self):
        return self.mind._internal.Occu.name

    def _occupation_hubs(self):
        return Lookup().GetHUBS(self.mind.mindtype, self._occupation())

    def AdjustWeights(self, sub, value):
        hubs = self._occupation_hubs()
        for hub in hubs:
            self.weights[hub] -= value * 0.1
        for hub in hubs:
            self.weights[hub] = 0.0 if self.weights[hub] < 0.0 else self.weights[hub]
        self.weights[sub] += value

    def GetIndex(self, subject):
        hubs = self._occupation_hubs()
        weights = [(hub, self.weights[hub]) for hub in hubs]
        total = sum(value for _, value in weights)
        count = 0.0
        result = 0.0
        for index, (hub, value) in enumerate(weights):
            count += value / total * 100.0
            if subject == self.GetSubject(count):
                break
            if index + 1 > len(weights) - 1:
                return -1.0
            next_area = weights[index + 1][1] / total * 100.0
            result = count + next_area / 2.0
        return result

    @staticmethod
    def _long_decision(unit):
        if not unit.IsDECISION():
            return ""
        data = unit.Data
        if data.startswith(CONST.lng_should):
            return CONST.LSUB_SHOULD
        if data.startswith(CONST.lng_what):
            return CONST.LSUB_WHAT
        return ""

    def GetSubject(self, value):
        if hasattr(value, "IsDECISION"):
            decision_subject = self._long_decision(value)
            if decision_subject:
                return decision_subject
            hub_index = value.HIget()
            if self._occupation() == "init":
                return "init"
        else:
            hub_index = float(value)

        hubs = self._occupation_hubs()
        # This intentionally uses every hub's weight, matching the source's
        # overloaded GetSubject implementation rather than only this occupation.
        total = sum(self.weights.values())
        if not hubs:
            return ""
        count = 0.0
        subject = ""
        for hub in hubs:
            count += self.weights[hub] / total * 100.0
            subject = hub
            if count >= hub_index:
                break
        return subject

    def UnitsPerHub(self, hub):
        return [unit for unit in self.mind.access.UNITS_ALL() if self.GetSubject(unit.HIget()) == hub]

    def UnitsPerOccupasionc(self):
        return [unit for unit in self.mind.access.UNITS_ALL() if unit.IsValid]

    @staticmethod
    def Max(values):
        result, maximum = "", -1.0
        for key, value in values.items():
            if value > maximum and value > 0.0:
                result, maximum = key, value
        return result
