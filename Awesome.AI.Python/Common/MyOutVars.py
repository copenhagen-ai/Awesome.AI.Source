"""Mind output values ported from ``Common/MyOutVars.cs`` (without ARC)."""
import math

from awesome_ai.Variables.Enums import HARDDOWN, LONGTYPE, STATE


class MyOutVars:
    FIELDS = (
        "ok", "cycles", "cycles_total", "vv_curr", "dv_curr", "actual_us_x",
        "actual_us_y", "num_units", "avg_area", "avg_radius",
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
        self.values = {}
        self.out_ready = False
        for field in self.FIELDS:
            setattr(self, field, "")
        self.ratio_yes_n = self.ratio_no_n = "0"

    @staticmethod
    def _text(value):
        return str(value)

    @staticmethod
    def _scientific(value):
        return f"{float(value):.3E}"

    def _set(self, **values):
        for field, value in values.items():
            setattr(self, field, self._text(value))
        self.values = {field: getattr(self, field) for field in self.FIELDS}

    def SetOut(self):
        if self.mind is None or self.mind.STATE == STATE.QUICKDECISION:
            return self.values

        mind = self.mind
        actual = mind.unit_actual
        ms = mind.mech.ms
        units = mind.access.UNITS_ALL()
        unit_count = len(units)
        area = (100.0 * 100.0) / unit_count if unit_count else 0.0
        radius = math.sqrt(area / math.pi) if area else 0.0
        occupation = mind._internal.Occu.name
        location = mind._long.GetResult(LONGTYPE.LOCATION)
        long_state = getattr(mind._long, "State", {}).get(LONGTYPE.LOCATION, 0)
        chat_answer = mind._long.GetResult(LONGTYPE.ANSWER) or self.chat_answer
        chat_subject = mind._long.GetResult(LONGTYPE.ASK) or self.chat_subject
        common_subject = mind.hub.GetSubject(actual) if actual is not None else ""
        position = mind.mech.PosXY()

        self._set(
            ok=mind.ok,
            cycles=mind.cycles,
            cycles_total=mind.cycles_all,
            error=mind.down._error,
            go_down="YES" if mind.down.d_res < 0.0 else "NO",
            ratio_yes_n=mind.down.Count(HARDDOWN.YES),
            ratio_no_n=mind.down.Count(HARDDOWN.NO),
            vv_curr=self._scientific(ms.vv_sym_curr),
            dv_curr=self._scientific(ms.dv_sym_curr),
            noise_norm=ms.vv_sym_90,
            actual_us_x=actual.UIget("will") if mind.environment.name == "LOCAL" and actual else -1,
            actual_us_y=actual.UIget("conflict") if mind.environment.name == "LOCAL" and actual else -1,
            num_units=unit_count,
            avg_area=area,
            avg_radius=radius,
            pain_truth_something=mind.pain_truth_something,
            position=position,
            epochs=mind.epochs,
            runtime=mind.bot.RUNTIME,
            whistle=getattr(mind, "result_whistle", ""),
            math=getattr(mind, "result_math", ""),
            common_hub_subject=common_subject or "",
            occu=occupation,
            location=location,
            loc_state="making a decision" if long_state > 0 else "just thinking",
            mood_pattern=mind.bot.pattern.name,
            mood_green=mind.mood.res_color.name == "GREEN",
            mood_norm=mind.mood.res_norm,
            monologue_det_result=mind.mono1.Result,
            monologue_det_subject=mind.mono1.Subject,
            monologue_det_relevance=mind.mono1.Relevance,
            monologue_lat_result=mind.mono2.Result,
            monologue_lat_subject=mind.mono2.Subject,
            monologue_lat_relevance=mind.mono2.Relevance,
            chat_answer=chat_answer,
            chat_subject=chat_subject,
        )
        self.out_ready = True
        return self.values
