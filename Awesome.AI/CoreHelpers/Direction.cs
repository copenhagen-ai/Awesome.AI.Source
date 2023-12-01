using Awesome.AI.Common;
using Awesome.AI.Core;
using static Awesome.AI.Helpers.Enums;

namespace Awesome.AI.CoreHelpers
{
    public class Ratio
    {
        TheMind mind;
        private Ratio() { }
        public Ratio(TheMind mind)
        {
            this.mind = mind;
        }

        public List<THECHOISE> ratio { get; set; } = new List<THECHOISE>();
        public int all_yes { get; set; } = 0;
        public int all_no { get; set; } = 0;

        public void Update(THECHOISE choise)
        {
            if (choise.IsYes())
                all_yes++;
            else
                all_no++;

            ratio.Add(choise);

            if (ratio.Count > mind.parms.lapses_total)
                ratio.RemoveAt(0);
        }

        public int CountYes()
        {
            int count = ratio.Where(z => z.IsYes()).Count();

            return count;
        }

        public int CountNo()
        {
            int count = ratio.Where(z => z.IsNo()).Count();

            return count;
        }
    }

    public class Direction
    {
        public Ratio ratio;
        public List<double> avg { get; set; } = new List<double>();
        public List<double> limit_periode { get; set; } = new List<double>();
        public List<double> limit { get; set; } = new List<double>();

        public double d_pos_x { get; set; }
        public double d_momentum { get; set; }

        TheMind mind;
        private Direction() { }
        public Direction(TheMind mind)
        {
            this.mind = mind;
            ratio = new Ratio(mind);
        }

        public THECHOISE Choise { get; set; } = THECHOISE.NO;
        private void SetChoise()
        {
            /*
             * >> this is the hack/cheat <<
             * "NO", is to say no to going downwards
             * */
            
            bool is_low = d_momentum <= 0.0d;

            Choise = is_low.TheHack1(mind) ? THECHOISE.NO : THECHOISE.YES;
        }

        /*
         * I call it SayNo, because GoRight/GoLeft is specific for the mechanics
         * ..and SayNo is indifferent of the direction
         * */
        public bool SayNo()
        {
            int count_no = ratio.CountNo();
            int count_yes = ratio.CountYes();

            bool say_no = count_no <= mind.parms.lim_ratio;    //true: more no, false: less no

            return say_no.TheHack2(mind);
        }

        public void Stat()
        {
            SetChoise();
                   
            ratio.Update(Choise);

            avg.Add(mind.parms._mech.lim.limit_result);
            if (avg.Count() > 1000)
                avg.RemoveAt(0);

            limit.Add(mind.parms._mech.lim.limit_result);
            if (limit.Count > mind.parms.lim_ratio)
                limit.RemoveAt(0);

            limit_periode.Add(mind.parms._mech.lim.limit_result);
            if (limit_periode.Count > 5000)
                limit_periode.RemoveAt(0);
        }/**/        
    }
}