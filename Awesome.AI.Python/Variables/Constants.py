from .Enums import HACKMODE, LONGTYPE, SELECTACTUAL, SELECTCURRENT, TRANSFER


class CONST:
    """Constants namespace. Values not present here can be added during parity work."""

    hack = HACKMODE.HACK
    LSUB_SHOULD = "long_decision_should"
    LSUB_WHAT = "long_decision_what"
    QSUB_SHOULD = "quick_decision_should"
    LDAT_LOC_SHOULD = "SHOULD_A"
    LDAT_ANS_SHOULD = "SHOULD_B"
    LDAT_ASK_SHOULD = "SHOULD_C"
    LDAT_LOC_WHAT_u1 = "WHAT_KITCHEN"
    LDAT_LOC_WHAT_u2 = "WHAT_BEDROOM"
    LDAT_LOC_WHAT_u3 = "WHAT_LIVINGROOM"
    LDAT_ANS_WHAT_u1 = "WHAT_im busy right now.."
    LDAT_ANS_WHAT_u2 = "WHAT_not right now.."
    LDAT_ANS_WHAT_u3 = "WHAT_talk later.."
    lng_should = "SHOULD_"
    lng_what = "WHAT_"
    lng_dec_basic = {LONGTYPE.LOCATION: "KITCHEN", LONGTYPE.ANSWER: "", LONGTYPE.ASK: ""}
    lng_dec_roberta = dict(lng_dec_basic)

    occupasions = ("socializing", "hobbys")
    sub_andrew = (
        "procrastination", "fembots", "power tools", "cars", "movies",
        "programming", "the weather", "life", "computers", "work",
    )
    sub_roberta = (
        "love", "macho machines", "music", "friends", "socializing",
        "dancing", "movies", "hobbys", "the weather", "having fun",
    )
    sub_basic = sub_roberta
    RS = 2.0
    FIRST_RUN = 5
    lng_dec_andrew = {LONGTYPE.LOCATION: "LIVINGROOM", LONGTYPE.ANSWER: "", LONGTYPE.ASK: ""}
    NUMBER_OF_UNITS = 10
    LOW_CREDIT = 1.0
    LOWCUT = 3
    AXES = ["will", "conflict", "commitment", "adaptation", "activation", "influence"]
    prop1_temperament = "temperament_1"
    prop2_temperament = "temperament_2"
    prop2_brain = "brain_2"
    prop3_brain = "brain_3"
    prop2_comm = "communication_2"
    prop3_comm = "communication_3"
    prop4_comm = "communication_4"
    LAPSES = 99
    STARTXY = 5.0
    LOWXY = 0.0
    HIGHXY = 10.0
    VERY_LOW = 1.0e-2
    GRAVITY = 9.81
    BASE_SCALE = 2.0 / 3.0
    SAMPLE20 = 20
    SAMPLE50 = 50
    MAX_CREDIT = 10.0
    MIN = .5
    MAX = 99.5
    MAX_HUBSPACE = 100.0
    DECAY = .99
    EPSILON1 = .001
    ETA = .9
    ALPHA = .5
    GAMMA = .01
    MAX_UNITS = 10
    UPD_CREDIT = 0.01
    EPSILON2 = 0.1
    MAX_PAIN_TRUTH_SOMETHING = 100.0
    HIST_TOTAL = 100
    REMEMBER = 50
    AGENT_DELAY_MS = 10
    transfer = TRANSFER.NONE
    select_curr = SELECTCURRENT.PYTH6
    select_act = SELECTACTUAL.DOMINANT

    @staticmethod
    def DECI_SUBJECT_CONTAINS(value: str) -> bool:
        return value in (CONST.LSUB_SHOULD, CONST.LSUB_WHAT, CONST.QSUB_SHOULD)
