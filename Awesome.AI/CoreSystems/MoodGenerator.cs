using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
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
                    case <= 3: mind.parms_current.pattern = PATTERN.MOODGENERAL; break;
                    case <= 6: mind.parms_current.pattern = PATTERN.MOODGOOD; break;
                    case <= 9: mind.parms_current.pattern = PATTERN.MOODBAD; break;
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

        public PATTERNCOLOR ResultColor { get; set; } = PATTERNCOLOR.RED;
        public double ResultMomentum { get; set; } = -1d;
        private List<double> Avg {  get; set; }
        private double a_low { get; set; } = 1000d;
        private double a_high { get; set; } = -1000d;
        PATTERN currentmood { get; set; } = PATTERN.NONE;
        public void MoodOK(bool _pro)
        {
            
            if (!CONST.SAMPLE200.RandomSample(mind)) 
                return;

            //if (currentmood != mind.parms_current.pattern)
            //    Reset();

            currentmood = mind.parms_current.pattern;

            double p_mom = mind.mech_current.p_curr;
            double d_mom = mind.mech_current.p_delta;

            Avg ??= new List<double>();
            Avg.Add(p_mom);
            if (Avg.Count > 1)
                Avg.RemoveAt(0);


            double p_high = mind.mech_current.m_out_high_c;
            double p_low = mind.mech_current.m_out_low_c;
            double d_high = mind.mech_current.d_out_high;
            double d_low = mind.mech_current.d_out_low;

            double avg = Avg.Average();
            
            if (avg > a_high) a_high = avg;
            if (avg < a_low) a_low = avg;

            double res = mind.calc.Normalize(avg, p_low, p_high, 10.0d, 90.0d);
            ResultMomentum = mind.calc.Normalize(avg, p_low, p_high, 10.0d, 90.0d);


            switch (currentmood)
            {
                case PATTERN.MOODGENERAL: 
                    ResultColor = PATTERNCOLOR.GREEN; 
                    break;
                case PATTERN.MOODGOOD: 
                    ResultColor = res >= 49.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; 
                    break;
                case PATTERN.MOODBAD: 
                    ResultColor = res <= 51.0d ? PATTERNCOLOR.GREEN : PATTERNCOLOR.RED; 
                    break;
            }
        }
    }
}
