using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Generators
{
    public class MoodGenerator
    {
        private TheMind mind;
        private MoodGenerator() { }

        public MoodGenerator(TheMind mind)
        {
            this.mind = mind;
        }

        private int Count { get; set; }
        public void Generate(bool _pro)
        {
            if (!_pro)
                return;

            int div = mind.bot.pattern == PATTERN.MOODBAD || mind.bot.pattern == PATTERN.MOODGENERAL? 5 : 10;

            if (Count >= div)
                Count = 0;

            if (Count == 2)
            {
                int rand = mind.rand.MyRandomInt(1, 79)[0];

                switch (rand / 10)
                {
                    case <= 1: mind.bot.pattern = PATTERN.MOODBAD; break;
                    case <= 3: mind.bot.pattern = PATTERN.MOODGENERAL; break;
                    case <= 8: mind.bot.pattern = PATTERN.MOODGOOD; break;
                    default: throw new Exception("MoodGenerator, Generate");
                }
            }

            Count++;
        }

        private void Reset()
        {
            a_low = 1000d;
            a_high = -1000d;
        }

        public PATTERNCOLOR ResColor { get; set; } = PATTERNCOLOR.RED;
        public double p_90 { get; set; } = -1d;
        private List<double> Avg { get; set; }
        private double a_low { get; set; } = 1000d;
        private double a_high { get; set; } = -1000d;
        PATTERN currentmood { get; set; } = PATTERN.NONE;
        public void MoodOK(bool _pro)
        {

            if (!CONST.SAMPLE200.RandomSample(mind))
                return;

            //if (currentmood != mind.parms_current.pattern)
            //    Reset();

            currentmood = mind.bot.pattern;

            double res = mind.mech_current.ms.vv_sym_90;
            p_90 = res;


            switch (currentmood)
            {
                case PATTERN.MOODGENERAL:
                    ResColor = PATTERNCOLOR.GREEN;
                    break;
                case PATTERN.MOODGOOD:
                    ResColor = res >= 45.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED;
                    break;
                case PATTERN.MOODBAD:
                    ResColor = res <= 55.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED;
                    break;
            }
        }
    }
}
