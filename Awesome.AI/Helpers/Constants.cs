using Awesome.AI.CoreHelpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.Helpers
{
    public class Constants
    {
        //these are HUB subjects
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

        //public const string make_decision_u1 = "MAKEYES";
        //public const string make_decision_u2 = "MAKENO";

        public const string location_should_decision_u1 = "SHOULDYES";
        public const string location_should_decision_u2 = "SHOULDNO";

        public const string location_what_decision_u1 = "WHATKITCHEN";
        public const string location_what_decision_u2 = "WHATBEDROOM";
        public const string location_what_decision_u3 = "WHATLIVINGROOM";

        public const string answer_should_decision_u1 = "ANSWERYES";
        public const string answer_should_decision_u2 = "ANSWERNO";

        public const string answer_what_decision_u1 = "WHATim busy right now..";
        public const string answer_what_decision_u2 = "WHATnot right now..";
        public const string answer_what_decision_u3 = "WHATtalk later..";

        public const string ask_should_decision_u1 = "ASKYES";
        public const string ask_should_decision_u2 = "ASKNO";

        public const double MIN = 0.5d;
        public const double MAX = 99.5d;
        public const double LOWXY = 0.0d;
        public const double HIGHXY = 10.0d;
        public const double STARTXY = 5.0d;

        public const double VERY_LOW = 1.0E-50;
        public const double GRAVITY = 9.81d;
        public const double GRAV_CONST = 6.674E-11d;
        public const double MAX_CREDIT = 10.0d;
        public const double LOW_CREDIT = 0.0d;

        public static readonly string[] subject_decision = new string[]
        { 
            "location_should_decision", "location_what_decision",
            "answer_should_decision", "answer_what_decision",
            "ask_should_decision",
        };

        //public const POSITION position = POSITION.NEW;
    }
}
