"""Bot definitions and factory ported from ``Factorys/BotFactory.cs``.

ARC is deliberately excluded from the Python port.
"""
from __future__ import annotations

from dataclasses import dataclass
from typing import TYPE_CHECKING

from awesome_ai.Variables.Constants import CONST
from awesome_ai.Variables.Enums import (
    LONGTYPE,
    LOGICTYPE,
    MECHANICS,
    MINDS,
    OCCUPASION,
    PATTERN,
    PROPS,
    TAGS,
    VALIDATION,
)

if TYPE_CHECKING:
    from awesome_ai.Core.TheMind import TheMind


@dataclass
class IBot:
    """Python data equivalent of the C# ``IBot`` property contract."""

    mindtype: MINDS
    logic: LOGICTYPE
    mech_low: MECHANICS
    props: PROPS
    lng_dec: dict[LONGTYPE, str]
    validation: VALIDATION
    tags: TAGS
    occupasion: OCCUPASION
    pattern: PATTERN
    RUNTIME: int


class Roberta(IBot):
    def __init__(self) -> None:
        super().__init__(
            MINDS.ROBERTA,
            LOGICTYPE.SHARED,
            MECHANICS.TUGOFWAR_LOW,
            PROPS.TEMPERAMENT,
            CONST.lng_dec_roberta,
            VALIDATION.BOTH,
            TAGS.ALL,
            OCCUPASION.DYNAMIC,
            PATTERN.MOODGENERAL,
            6 // 2,
        )


class Andrew(IBot):
    def __init__(self) -> None:
        super().__init__(
            MINDS.ANDREW,
            LOGICTYPE.SHARED,
            MECHANICS.TUGOFWAR_LOW,
            PROPS.TEMPERAMENT,
            CONST.lng_dec_andrew,
            VALIDATION.BOTH,
            TAGS.ALL,
            OCCUPASION.DYNAMIC,
            PATTERN.MOODGENERAL,
            6 // 2,
        )


class Basic(IBot):
    def __init__(self) -> None:
        super().__init__(
            MINDS.BASIC,
            LOGICTYPE.SHARED,
            MECHANICS.TUGOFWAR_LOW,
            PROPS.TEMPERAMENT,
            CONST.lng_dec_basic,
            VALIDATION.BOTH,
            TAGS.ALL,
            OCCUPASION.DYNAMIC,
            PATTERN.MOODGENERAL,
            120 // 2,
        )


class BotFactory:
    def __init__(self, mind: TheMind) -> None:
        self.mind = mind
        self.mindtype = mind.mindtype

    def GetBot(self) -> IBot:
        if self.mindtype == MINDS.ROBERTA:
            return Roberta()
        if self.mindtype == MINDS.ANDREW:
            return Andrew()
        if self.mindtype == MINDS.BASIC:
            return Basic()
        raise Exception("Bots, GetBot")
