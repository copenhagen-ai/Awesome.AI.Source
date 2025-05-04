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
        public PERIODDOWN DownPeriod { get { return RatioNoise.PeriodDown(mind); } }

        public List<HARDDOWN> RatioNoise { get; set; }

        private TheMind mind;
        private Direction() { }
        public Direction(TheMind mind)
        {
            this.mind = mind;
            RatioNoise = new List<HARDDOWN>();
        }

        public void Update()
        {
            if (mind.z_current != "z_noise")
                return;
            
            RatioNoise.Add(DownHard);

            if (RatioNoise.Count > CONST.LAPSES)
                RatioNoise.RemoveAt(0);            
        }

        public int Count(HARDDOWN choise/*, bool is_noise*/)
        {
            int count = 0;
            switch (choise)
            {
                case HARDDOWN.YES: count = RatioNoise.Where(z => z.IsYes()).Count(); break;
                case HARDDOWN.NO: count = RatioNoise.Where(z => z.IsNo()).Count(); break;
            }

            return count;
        }
    }
}