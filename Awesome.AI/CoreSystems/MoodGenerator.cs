using Awesome.AI.Core;
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
        private List<double> Avg {  get; set; }
        public void MoodOK(bool _pro)
        {
            double p_mom = mind.mech[mind.current].p_curr;

            Avg ??= new List<double>();
            Avg.Add(p_mom);
            if (Avg.Count > 100)
                Avg.RemoveAt(0);

            if (!mind.calc.IsRandomSample(200, 10)) 
                return;

            double p_high = mind.mech[mind.current].m_out_high_c;
            double p_low = mind.mech[mind.current].m_out_low_c;

            double avg = Avg.Average();
            double res = mind.calc.Normalize(avg, p_low, p_high, 0.0d, 100.0d);

            PATTERN currentmood = mind.parms[mind.current].version;

            switch (currentmood)
            {
                case PATTERN.MOODGENERAL: Result = PATTERNCOLOR.GREEN; break;
                case PATTERN.MOODGOOD: Result = res >= 50.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; break;
                case PATTERN.MOODBAD: Result = res < 50.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; break;
            }
        }
    }
}
