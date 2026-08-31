from dataclasses import dataclass
import json


@dataclass
class MyMeters:
    units_added: int = 0
    units_removed: int = 0


class MyStats:
    def __init__(self):
        self.hits = {}
        self.units = {}
    def HitsJson(self): return json.dumps(self.hits, separators=(",", ":"))
    def UnitsJson(self): return json.dumps(self.units, separators=(",", ":"))
