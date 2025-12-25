namespace Awesome.AI.Variables
{
    public class Enums
    {
        public enum VALUE { TRUE, FALSE, EMPTY }
        public enum CASE { CHATASKED, CHATRESET, NONE }
        public enum MINDS { ROBERTA, ANDREW }
        public enum PATTERNCOLOR { GREEN, RED }
        public enum PATTERN { NONE, MOODGENERAL, MOODGOOD, MOODBAD }
        public enum STATE { JUSTRUNNING, QUICKDECISION }
        public enum TONE { HIGH, LOW, MID, RANDOM }
        public enum UNITTYPE { JUSTAUNIT, LDECISION, QDECISION, IDLE, MIN, MAX }
        public enum LONGTYPE { LOCATION, ANSWER, ASK, NONE }
        public enum VALIDATION { BOTH, EXTERNAL, INTERNAL }
        public enum TAGS { ALL, EVEN }
        public enum OCCUPASION { FIXED, DYNAMIC }
        public enum MECHANICS { TUGOFWAR_LOW, BALLONHILL_LOW, CIRCUIT_1_LOW, CIRCUIT_2_LOW, GRAVITY_HIGH, TUGOFWAR_HIGH, BALLONHILL_HIGH }
        public enum LIMITTYPE { SIMPLE, SIGMOID, CHANCE }
        public enum MOOD { GOOD, BAD }
        public enum LOGICTYPE { CLASSICAL, PROBABILITY, QUBIT }
        public enum HACKMODE { HACK, NOHACK }
        public enum HARDDOWN { YES, NO }
        public enum FUZZYDOWN { VERYYES, YES, MAYBE, NO, VERYNO }
        public enum PERIODDOWN { YES, NO }
        public enum ORDER { NONE, BYINDEX, BYVARIABLE }
        public enum PROPS { COMMUNICATION, BRAINWAVE }
    }
}
