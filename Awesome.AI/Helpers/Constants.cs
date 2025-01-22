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
        public const string andrew_s7 = "websites";
        public const string andrew_s8 = "existence";
        public const string andrew_s9 = "termination";
        public const string andrew_s10 = "data";

        public const string roberta_s1 = "love";
        public const string roberta_s2 = "macho machines";
        public const string roberta_s3 = "music";
        public const string roberta_s4 = "friends";
        public const string roberta_s5 = "socializing";
        public const string roberta_s6 = "dancing";
        public const string roberta_s7 = "movies";
        public const string roberta_s8 = "existence";
        public const string roberta_s9 = "termination";
        public const string roberta_s10 = "programming";

        public const string should_decision_u1 = "SHOULDYES";
        public const string should_decision_u2 = "SHOULDNO";

        public const string make_decision_u1 = "MAKEYES";
        public const string make_decision_u2 = "MAKENO";

        public const string what_decision_u1 = "WHATKITCHEN";
        public const string what_decision_u2 = "WHATBEDROOM";
        public const string what_decision_u3 = "WHATLIVINGROOM";

        public const double MIN = 0.5d;
        public const double MAX = 99.5d;

        public const double VERY_LOW = 1.0E-50;
        public const double GRAVITY = 9.81d;

        public static readonly string[] subject_decision = new string[]{ "should_decision", "what_decision" };
    }
}
