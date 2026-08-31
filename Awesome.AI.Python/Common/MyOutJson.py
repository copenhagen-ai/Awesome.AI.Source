"""JSON output model ported from ``Common/MyOutJson.cs`` (without ARC)."""
import json


class JsonObject(dict):
    """Ordered JSON field container matching the C# output object."""


class MyOutJson:
    FIELDS = (
        "ok", "cycles", "cycles_total", "vv_curr", "dv_curr",
        "pain_truth_something", "position", "ratio_yes_n", "ratio_no_n",
        "go_down", "epochs", "runtime", "occu", "location", "loc_state",
        "chat_answer", "chat_subject", "whistle", "math",
        "monologue_det_result", "monologue_det_subject", "monologue_det_relevance",
        "monologue_lat_result", "monologue_lat_subject", "monologue_lat_relevance",
        "mood_pattern", "mood_green", "mood_norm", "noise_norm", "error",
        "common_hub_subject",
    )

    def __init__(self, mind=None):
        self.mind = mind
        self.obj = JsonObject()
        self.json = ""

    def SetObj(self):
        self.obj = JsonObject()
        if self.mind is None:
            return
        output = self.mind.o_vars
        values = getattr(output, "values", {})
        for field in self.FIELDS:
            self.obj[field] = getattr(output, field, values.get(field, ""))

    def GetJson(self, _pro=True):
        self.SetObj()
        self.json = json.dumps(self.obj, separators=(",", ":"))
        return self.json
