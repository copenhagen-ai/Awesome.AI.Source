from enum import Enum, auto


class _AutoName(Enum):
    def _generate_next_value_(name, start, count, last_values):
        return name


def _enum(name: str, members: str):
    return Enum(name, {member: member for member in members.split()}, type=_AutoName)


VALUE = _enum("VALUE", "TRUE FALSE EMPTY")
CASE = _enum("CASE", "CHATASKED CHATRESET NONE")
MINDS = _enum("MINDS", "ROBERTA ANDREW BASIC")
ENV = _enum("ENV", "LOCAL SERVER")
PATTERNCOLOR = _enum("PATTERNCOLOR", "GREEN RED")
PATTERN = _enum("PATTERN", "NONE MOODGENERAL MOODGOOD MOODBAD")
STATE = _enum("STATE", "JUSTRUNNING QUICKDECISION")
TONE = _enum("TONE", "HIGH LOW MID RANDOM")
UNITTYPE = _enum("UNITTYPE", "JUSTAUNIT LDECISION QDECISION IDLE MIN MAX")
LONGTYPE = _enum("LONGTYPE", "LOCATION ANSWER ASK NONE")
VALIDATION = _enum("VALIDATION", "BOTH EXTERNAL INTERNAL")
TAGS = _enum("TAGS", "ALL EVEN")
OCCUPASION = _enum("OCCUPASION", "FIXED DYNAMIC")
MECHANICS = _enum("MECHANICS", "MECH_OTHER_LOW TUGOFWAR_LOW BALLONHILL_LOW CIRCUIT_1_LOW CIRCUIT_2_LOW")
LOGICTYPE = _enum("LOGICTYPE", "CLASSICAL PROBABILITY QUBIT SHARED")
SELECTCURRENT = _enum("SELECTCURRENT", "PYTH2 PYTH6 OTHER")
SELECTACTUAL = _enum("SELECTACTUAL", "DOMINANT OTHER")
HACKMODE = _enum("HACKMODE", "HACK NOHACK")
HARDDOWN = _enum("HARDDOWN", "YES NO")
FUZZYDOWN = _enum("FUZZYDOWN", "VERYYES YES MAYBE NO VERYNO")
PERIODDOWN = _enum("PERIODDOWN", "YES NO")
ORDER = _enum("ORDER", "NONE BYINDEX BYVARIABLE")
PROPS = _enum("PROPS", "TEMPERAMENT COMMUNICATION BRAINWAVE")
FILTERUNIT = _enum("FILTERUNIT", "NONE CURRENT ACTUAL")
FILTERTYPE = _enum("FILTERTYPE", "ONE TWO THREE")
ACTION = _enum("ACTION", "ACTION DECLINE")
TRANSFER = _enum("TRANSFER", "NONE LOGISTIC OTHER")
ARC = _enum("ARC", "USE DONTUSE")


class Enums:
    """Compatibility container matching the C# nested-enum class."""


for _name, _value in tuple(globals().items()):
    if isinstance(_value, type) and issubclass(_value, Enum) and _name != "_AutoName":
        setattr(Enums, _name, _value)

