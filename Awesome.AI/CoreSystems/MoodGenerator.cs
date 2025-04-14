using Awesome.AI.Core;
using Awesome.AI.Interfaces;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreSystems
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

            if (Count >= 10) 
                Count = 0;

            if(Count == 5) 
            {
                int _rand = mind.rand.MyRandomInt(1, 9)[0];

                switch (_rand)
                {
                    case <= 0: throw new Exception("MoodGenerator, Generate");                        
                    case <= 3: mind.parms[mind.current].version = PATTERN.MOODGENERAL; break;
                    case <= 6: mind.parms[mind.current].version = PATTERN.MOODGOOD; break;
                    case <= 9: mind.parms[mind.current].version = PATTERN.MOODBAD; break;
                    default: throw new Exception("MoodGenerator, Generate");
                }
            }

            Count++;
        }

        public PATTERNCOLOR Result { get; set; } = PATTERNCOLOR.RED;
        public void MoodOK(bool _pro)
        {
            if (!_pro)
                return;

            double d_mom = mind.mech[mind.current].p_delta;
            double d_high = mind.mech[mind.current].d_out_high;
            double d_low = mind.mech[mind.current].d_out_low;

            double res = mind.calc.NormalizeRange(d_mom, d_low, d_high, 0.0d, 100.0d);

            PATTERN currentmood = mind.parms[mind.current].version;

            if (mind.mindtype == MINDS.ROBERTA)
            {

                switch (currentmood)
                {
                    case PATTERN.MOODGENERAL: Result = PATTERNCOLOR.GREEN; break;
                    case PATTERN.MOODGOOD: Result = res >= 50.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; break;
                    case PATTERN.MOODBAD: Result = res < 50.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; break;
                }
            }

            if(mind.mindtype == MINDS.ANDREW)
            {
                switch (currentmood)
                {
                    case PATTERN.MOODGENERAL: Result = PATTERNCOLOR.GREEN; break;
                    case PATTERN.MOODBAD: Result = res >= 50.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; break;
                    case PATTERN.MOODGOOD: Result = res < 50.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; break;
                }
            }

        }
    }
}
