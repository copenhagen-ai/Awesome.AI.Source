using Awesome.AI.Common;
using Awesome.AI.Core;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.CoreHelpers
{
    public class Direction
    {
        TheMind mind;
        private Direction() { }
        public Direction(TheMind mind)
        {
            this.mind = mind;
        }

        public THECHOISE Choise { get; set; } = THECHOISE.NO;
        public List<THECHOISE> ratio { get; set; } = new List<THECHOISE>();
        //public int all_yes { get; set; } = 0;
        //public int all_no { get; set; } = 0;

        public void Update()
        {
            SetChoise();                       
            UpdateRatio();
        }

        private void SetChoise()
        {
            /*
             * >> this is the hack/cheat <<
             * "NO", is to say no to going downwards
             * */
            
            bool is_low = mind.mech.momentum <= 0.0d;

            Choise = is_low.TheHack1(mind) ? THECHOISE.NO : THECHOISE.YES;
        }

        public void UpdateRatio()
        {
            //if (Choise.IsYes())
            //    all_yes++;
            //else
            //    all_no++;

            ratio.Add(Choise);

            if (ratio.Count > mind.parms.lapses_total)
                ratio.RemoveAt(0);
        }

        public int Count(THECHOISE choise)
        {
            int count = 0;
            switch (choise)
            {
                case THECHOISE.YES: count = ratio.Where(z => z.IsYes()).Count(); break;
                case THECHOISE.NO:  count = ratio.Where(z => z.IsNo()).Count(); break;
            }

            return count;
        }

        /*
         * I call it SayNo, because GoRight/GoLeft is specific for the mechanics
         * ..and SayNo is indifferent of the direction
         * */
        //public bool SayNo()
        //{
        //    int count_no = ratio.CountNo();
        //    int count_yes = ratio.CountYes();

        //    bool say_no = count_no <= mind.parms.ratio;    //true: more no, false: less no

        //    return say_no.TheHack2(mind);
        //}
    }
}