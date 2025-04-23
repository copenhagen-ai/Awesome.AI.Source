using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{
    public class Direction
    {
        public HARDDOWN DownHard { get { return mind.mech_current.HardMom; } }
        public FUZZYDOWN DownFuzzy { get { return mind.mech_current.FuzzyMom; } }
        public PERIODDOWN DownPeriod { get { return RatioCurrent.PeriodDown(mind); } }

        public List<HARDDOWN> RatioCurrent { get; set; }
        public List<HARDDOWN> RatioNoise { get; set; }

        private TheMind mind;
        private Direction() { }
        public Direction(TheMind mind)
        {
            this.mind = mind;
            RatioCurrent = new List<HARDDOWN>();
            RatioNoise = new List<HARDDOWN>();
        }

        public void Update()
        {
            if (mind.current == "noise")
            {
                RatioNoise.Add(DownHard);

                if (RatioNoise.Count > CONST.LAPSES)
                    RatioNoise.RemoveAt(0);
            }

            if (mind.current == "mech")
            {
                RatioCurrent.Add(DownHard);

                if (RatioCurrent.Count > CONST.LAPSES)
                    RatioCurrent.RemoveAt(0);
            }
        }

        public int Count(HARDDOWN choise, bool is_noise)
        {
            int count = 0;
            switch (choise)
            {
                case HARDDOWN.YES: count = is_noise ? RatioNoise.Where(z => z.IsYes()).Count() : RatioCurrent.Where(z => z.IsYes()).Count(); break;
                case HARDDOWN.NO: count = is_noise ? RatioNoise.Where(z => z.IsNo()).Count() : RatioCurrent.Where(z => z.IsNo()).Count(); break;
            }

            return count;
        }
    }
}