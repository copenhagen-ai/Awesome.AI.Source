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

        public const string basic_s1 = "love";
        public const string basic_s2 = "macho machines";
        public const string basic_s3 = "music";
        public const string basic_s4 = "friends";
        public const string basic_s5 = "socializing";
        public const string basic_s6 = "dancing";
        public const string basic_s7 = "movies";
        public const string basic_s8 = "hobbys";
        public const string basic_s9 = "the weather";
        public const string basic_s10 = "having fun";

        public static string[] sub_roberta = { CONST.roberta_s1, CONST.roberta_s2, CONST.roberta_s3, CONST.roberta_s4, CONST.roberta_s5, CONST.roberta_s6, CONST.roberta_s7, CONST.roberta_s8, CONST.roberta_s9, CONST.roberta_s10 };
        public static string[] sub_andrew = { CONST.andrew_s1, CONST.andrew_s2, CONST.andrew_s3, CONST.andrew_s4, CONST.andrew_s5, CONST.andrew_s6, CONST.andrew_s7, CONST.andrew_s8, CONST.andrew_s9, CONST.andrew_s10 };
        public static string[] sub_basic = { CONST.basic_s1, CONST.basic_s2, CONST.basic_s3, CONST.basic_s4, CONST.basic_s5, CONST.basic_s6, CONST.basic_s7, CONST.basic_s8, CONST.basic_s9, CONST.basic_s10 };

        public const string axis_1_brain = "will";
        public const string axis_2_brain = "attention";
        public const string axis_3_brain = "readiness";

        public const string axis_1_comm = "will";
        public const string axis_2_comm = "opinion";
        public const string axis_3_comm = "temporality";
        public const string axis_4_comm = "abstraction";

        /*
         * decision values
         * */

        public const string lng_should = "SHOULD_";
        public const string lng_what = "WHAT_";

        public const string q_yes = "QYES";
        public const string q_no = "QNO";

        public const string LDAT_LOC_SHOULD = "SHOULD_A";
        public const string LDAT_ANS_SHOULD = "SHOULD_B";
        public const string LDAT_ASK_SHOULD = "SHOULD_C";

        public const string LDAT_LOC_WHAT_u1 = "WHAT_KITCHEN";
        public const string LDAT_LOC_WHAT_u2 = "WHAT_BEDROOM";
        public const string LDAT_LOC_WHAT_u3 = "WHAT_LIVINGROOM";

        public const string LDAT_ANS_WHAT_u1 = "WHAT_im busy right now..";
        public const string LDAT_ANS_WHAT_u2 = "WHAT_not right now..";
        public const string LDAT_ANS_WHAT_u3 = "WHAT_talk later..";

        public const string LSUB_SHOULD = "long_decision_should";
        public const string LSUB_WHAT = "long_decision_what";
        public const string QSUB_SHOULD = "quick_decision_should";
        
        public static bool DECI_SUBJECT_CONTAINS(string str) 
        { 
            return LSUB_SHOULD == str || LSUB_WHAT == str || QSUB_SHOULD == str; 
        }

        public static readonly Dictionary<LONGTYPE, string> lng_dec_basic = new Dictionary<LONGTYPE, string>
        {
            { LONGTYPE.LOCATION, "KITCHEN" },
            { LONGTYPE.ANSWER, "" },
            { LONGTYPE.ASK, "" }
        };

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
        public const double UPD_CREDIT = 0.01d;          //credit update
        public const double MAX_HUBSPACE = 100.0;

        public const double DECAY = 0.99d;
        public const double EPSILON = 0.01d;                //UNITTYPE removal
        public const double ETA = 0.9d;                    //learningrate
        public const double ALPHA = 0.5d;                 //distance
        public const double GAMMA = 0.01d;                  //keep below 0.5

        public const double GRAVITY = 9.81d;
        public const double GRAV_CONST = 6.674E-11d;        //6.67430E-11;

        public const double BASE_SCALE = 2d / 3d;
        public const double MAX_PAIN = 100.0d;              //connected x
        public const double VERY_LOW = 1.0E-2;              //connected x
        public const double RS = 2.0;                       //Schwarzschild radius
        public const double LAPSES = 99d;                   //yesno ratio : reaction time in cycles
        public const double RATIO = 50d;
        public const int FIRST_RUN = 5;
        public const int LOWCUT = 3;        
        public const int NUMBER_OF_UNITS = 10;
        public const int MAX_UNITS = 10;

        public const int SAMPLE20 = 20;
        public const int SAMPLE50 = 50;
        public const int SAMPLE100 = 100;
        public const int SAMPLE200 = 200;
        public const int SAMPLEHIGH = 80000;

        [Obsolete]
        public const HACKMODE hack = HACKMODE.HACK;

        public const TRANSFER transfer = TRANSFER.NONE;
        public const SELECTCURRENT select_c = SELECTCURRENT.PYTH;
        public const SELECTACTUAL select_a = SELECTACTUAL.DOMINANT;

        public const bool AGENT_USE_TIMER = false;
        public const int AGENT_DELAY_MS = 10;
        public const int AGENT_MICRO_SEC = 10000;           //call micro timer every 1000µs (1ms)

        public const int HIST_TOTAL = 100;                  //the number of UNITS???
        public const int REMEMBER = 50;                     //for stats                
    }
}
