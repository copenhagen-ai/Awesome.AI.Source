using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.CoreInternals
{
    public class Direction
    {
        public HARDDOWN DownHard { get { return mind.mech.HardMom; } }
        public FUZZYDOWN DownFuzzy { get { return mind.mech.FuzzyMom; } }
        public PERIODDOWN DownPeriod { get { return Ratio.PeriodDown(mind); } }

        public List<HARDDOWN> Ratio { get; set; }

        private TheMind mind;
        private Direction() { }
        public Direction(TheMind mind)
        {
            this.mind = mind;
            Ratio = new List<HARDDOWN>();
        }

        public void Update()
        {
            Ratio.Add(DownHard);

            if (Ratio.Count > Constants.LAPSES)
                Ratio.RemoveAt(0);
        }

        public int Count(HARDDOWN choise)
        {
            int count = 0;
            switch (choise)
            {
                case HARDDOWN.YES: count = Ratio.Where(z => z.IsYes()).Count(); break;
                case HARDDOWN.NO: count = Ratio.Where(z => z.IsNo()).Count(); break;
            }

            return count;
        }
    }
}