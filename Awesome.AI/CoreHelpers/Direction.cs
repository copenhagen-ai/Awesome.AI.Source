using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Helpers;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.CoreHelpers
{
    public class Direction
    {
        public HARDCHOICE ChoiceHard { get { return mind.mech.TheChoice; } }
        public SOFTCHOICE ChoiceSoft { get { return mind.mech.FuzzyMom; } }
        public bool ChoicePeriod { get { return Ratio.NoOverTime(mind); } }

        public List<HARDCHOICE> Ratio { get; set; }

        private TheMind mind;
        private Direction() { }
        public Direction(TheMind mind)
        {
            this.mind = mind;
            Ratio = new List<HARDCHOICE>();
        }
        
        public void Update()
        {
            Ratio.Add(ChoiceHard);

            if (Ratio.Count > Constants.LAPSES_TOTAL)
                Ratio.RemoveAt(0);
        }

        public int Count(HARDCHOICE choise)
        {
            int count = 0;
            switch (choise)
            {
                case HARDCHOICE.YES: count = Ratio.Where(z => !z.IsNo()).Count(); break;
                case HARDCHOICE.NO:  count = Ratio.Where(z => z.IsNo()).Count(); break;
            }

            return count;
        }        
    }
}