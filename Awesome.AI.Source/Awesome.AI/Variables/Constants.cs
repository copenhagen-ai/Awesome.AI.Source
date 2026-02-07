using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Variables
{
    public class CONST
    {
        /*
         * these are HUB subjects
         * */

        public const string andrew_s1 = "procrastination";
        public const string andrew_s2 = "fembots";
        public const string andrew_s3 = "power tools";
        public const string andrew_s4 = "cars";
        public const string andrew_s5 = "movies";
        public const string andrew_s6 = "programming";
        public const string andrew_s7 = "the weather";
        public const string andrew_s8 = "life";
        public const string andrew_s9 = "computers";
        public const string andrew_s10 = "work";

        public const string roberta_s1 = "love";
        public const string roberta_s2 = "macho machines";
        public const string roberta_s3 = "music";
        public const string roberta_s4 = "friends";
        public const string roberta_s5 = "socializing";
        public const string roberta_s6 = "dancing";
        public const string roberta_s7 = "movies";
        public const string roberta_s8 = "hobbys";
        public const string roberta_s9 = "the weather";
        public const string roberta_s10 = "having fun";

        public static string[] sub_roberta = { CONST.roberta_s1, CONST.roberta_s2, CONST.roberta_s3, CONST.roberta_s4, CONST.roberta_s5, CONST.roberta_s6, CONST.roberta_s7, CONST.roberta_s8, CONST.roberta_s9, CONST.roberta_s10 };
        public static string[] sub_andrew = { CONST.andrew_s1, CONST.andrew_s2, CONST.andrew_s3, CONST.andrew_s4, CONST.andrew_s5, CONST.andrew_s6, CONST.andrew_s7, CONST.andrew_s8, CONST.andrew_s9, CONST.andrew_s10 };

        /*
         * decision values
         * */

        public const string location_should = "A";
        public const string answer_should = "B";
        public const string ask_should = "C";

        public const string location_what_u1 = "WHATKITCHEN";
        public const string location_what_u2 = "WHATBEDROOM";
        public const string location_what_u3 = "WHATLIVINGROOM";

        public const string answer_what_u1 = "WHATim busy right now..";
        public const string answer_what_u2 = "WHATnot right now..";
        public const string answer_what_u3 = "WHATtalk later..";

        public const string quick_deci_should_yes = "QYES";
        public const string quick_deci_should_no = "QNO";


        public const string DECI_SUBJECT_A = "long_decision_should";
        public const string DECI_SUBJECT_B = "long_decision_what";
        public const string DECI_SUBJECT_C = "quick_decision_should";
        public static bool DECI_SUBJECT_CONTAINS(string str) { return DECI_SUBJECT_A == str || DECI_SUBJECT_B == str || DECI_SUBJECT_C == str; }

        //public static readonly Dictionary<string, int[]> DECISIONS_A = new Dictionary<string, int[]>
        //{
        //    { "WHISTLE", new int[]{ 5, 4 } },
        //};

        //public static readonly Dictionary<string, int[]> DECISIONS_R = new Dictionary<string, int[]>
        //{
        //    { "WHISTLE", new int[]{ 5, 4 } },
        //};

        //public static readonly Dictionary<string, string> lng_dec_roberta = new Dictionary<string, string>
        //{
        //    { "location", "KITCHEN" },
        //    { "answer", "" },
        //    { "ask", "" }
        //};

        //public static readonly Dictionary<string, string> lng_dec_andrew = new Dictionary<string, string>
        //{
        //    { "location", "LIVINGROOM" },
        //    { "answer", "" },
        //    { "ask", "" }
        //};

        public static readonly Dictionary<LONGTYPE, string> lng_dec_roberta = new Dictionary<LONGTYPE, string>
        {
            { LONGTYPE.LOCATION, "KITCHEN" },
            { LONGTYPE.ANSWER, "" },
            { LONGTYPE.ASK, "" }
        };

        public static readonly Dictionary<LONGTYPE, string> lng_dec_andrew = new Dictionary<LONGTYPE, string>
        {
            { LONGTYPE.LOCATION, "LIVINGROOM" },
            { LONGTYPE.ANSWER, "" },
            { LONGTYPE.ASK, "" }
        };

        /*
         * system constants
         * */

        public const double STARTXY = 5.0d;
        public const double LOWXY = 0.0d;
        public const double HIGHXY = 10.0d;
        public const double MIN = 0.5d;
        public const double MAX = 99.5d;
        public const double LOW_CREDIT = 1.0d;
        public const double MAX_CREDIT = 10.0d;
        public const double UPD_CREDIT = 0.0001d;          //credit update
        public const double MAX_HUBSPACE = 100.0;

        public const double ETA = 0.05d;                    //learningrate
        public const double ALPHA = 0.001d;                 //distance
        public const int ACTIVATOR = 8000;                  //randomness, turn it way down

        public const double GRAVITY = 9.81d;
        public const double GRAV_CONST = 6.674E-11d;        //6.67430E-11;

        public const double BASE_REDUCTION = 2d / 3d;
        public const double MAX_PAIN = 100.0d;              //connected x
        public const double VERY_LOW = 1.0E-2;              //connected x
        public const double RS = 2.0;                       //Schwarzschild radius
        public const double LAPSES = 99d;                   //yesno ratio : reaction time in cycles
        public const double RATIO = 50d;
        public const int FIRST_RUN = 5;
        public const int RUNTIME = 3;                       //minutes
        public const int LOWCUT = 3;        
        public const int NUMBER_OF_UNITS = 10;

        public const int SAMPLE20 = 20;
        public const int SAMPLE50 = 50;
        public const int SAMPLE100 = 100;
        public const int SAMPLE200 = 200;
        public const int SAMPLEHIGH = 80000;

        public const LOGICTYPE Logic = LOGICTYPE.PROBABILITY;
        public const MECHANICS MechType = MECHANICS.TUGOFWAR_LOW;
        public const HACKMODE hack = HACKMODE.HACK;

        public const int MICRO_SEC = 10000;                 //call micro timer every 1000µs (1ms)
        public const int HIST_TOTAL = 100;                  //the number of UNITS???
        public const int REMEMBER = 50;                    //for stats
    }
}
