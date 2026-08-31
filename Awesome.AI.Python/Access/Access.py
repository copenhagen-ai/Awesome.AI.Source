from dataclasses import dataclass, field
from typing import Any
from awesome_ai.Variables.Enums import CASE, MINDS, VALUE


class AccessApi:
    pass


@dataclass
class ApiRequestSetup:
    bots: list[MINDS] = field(default_factory=list)


@dataclass
class ApiRequestUpdate:
    mindtype: MINDS = MINDS.BASIC
    _case: CASE = CASE.NONE
    _value: VALUE = VALUE.EMPTY


@dataclass
class ApiRequestAnswer:
    mindtype: MINDS = MINDS.BASIC


@dataclass
class ApiInstance:
    o_json: str = ""
    s_hits_json: str = ""
    s_units_json: str = ""
    o_vars: Any = None
    s_vars: Any = None
    mindtype: MINDS = MINDS.BASIC

