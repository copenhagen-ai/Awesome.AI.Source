using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.CoreSystems;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Awesome.AI.Core
{
    public class Down
    {
        public double Dir
        {
            get
            {
                double will = mind.mech.ms.dv_sym_curr.Norm1(mind);
                double zero = 0.0.Norm1(mind);
                
                return will <= zero ? -1.0d : 1.0d;
            } 
        }
                
        public int Error { get; set; }
        public List<double> Ratio { get; set; }
        private List<bool> Errors { get; set; }

        private TheMind mind;
        private Down() { }
        public Down(TheMind mind)
        {
            this.mind = mind;

            Ratio = new List<double>();
            Errors = new List<bool>();
        }

        public void Update()
        {
            Continous();

            //code: before or after?
            Ratio.Add(Dir);
            if (Ratio.Count > CONST.LAPSES)
                Ratio.RemoveAt(0);            
        }

        public int Count(HARDDOWN dir)
        {
            int count = 0;
            switch (dir)
            {
                case HARDDOWN.YES: count = Ratio.Where(z => z <= 0.0d).Count(); break;
                case HARDDOWN.NO: count = Ratio.Where(z => z > 0.0d).Count(); break;
            }

            return count;
        }

        public void SetError(bool err)
        {
            Errors.Add(err);
            if (Errors.Count > 100)
                Errors.RemoveAt(0);

            Error = Errors.Count(x => x == true);
        }
        
        public void Continous()
        {
            /*
             * this can be extended with a mechanism to alter between the two
             * thereby expressing levels of social entanglement
             * */

            double d_curr = mind.mech.ms.dv_sym_curr;
            double d_zero = d_curr.Norm0(mind);
            double d_save = d_curr.Norm0(mind);

            if (mind.bot.logic == LOGICTYPE.PROBABILITY && Probability(d_curr, mind)/* && NoInertia() && NoMomentum()*/)
                d_zero *= -1.0d;

            if (mind.bot.logic == LOGICTYPE.SHARED && Shared(d_curr, mind)/* && NoInertia()*/ && NoMomentum())
                d_zero *= -1.0d;

            bool flip = d_save != d_zero;
            DoFlip(flip, d_curr, out d_curr);
            SetError(flip);
        }

        private int i_decay { get; set; }
        private bool NoInertia()
        {
            /*
             * simple
             * cancels out some of the direction changes
             * */

            return true;

            i_decay++;

            double vv_curr = mind.mech.ms.vv_sym_curr;
            double dv_curr = mind.mech.ms.dv_sym_curr;
            double lim = mind.mech.mp.inertia_lim;

            bool inertia = Math.Abs(vv_curr + dv_curr) < lim;

            if (!inertia && i_decay > 100)
                i_decay = 0;

            return !inertia && i_decay < 10;
        }

        double abs_max {  get; set; }
        private bool NoMomentum()
        {
            /*
             * simple
             * cancels out some of the direction changes
             * */

            //return true;
            //double sign = Math.Sign(mind.mech_current.ms.fsta_sym + mind.mech_current.ms.fdyn_sym);

            double vel = mind.mech.ms.vv_sym_curr;
            double mass = mind.mech.ms.m1_sym + mind.mech.ms.m2_sym;
            double mom = mass * vel;

            if (mom >= 0.0)
                ;

            if (mom < 0.0)
                ;

            double abs = Math.Abs(mom);

            if(abs > abs_max)
                abs_max = abs;

            bool res = abs < abs_max / 10.0d;

            return res;
        }

        private void DoFlip(bool flip, double d_curr, out double _out)
        {
            _out = flip ? 
                d_curr.Flip(mind) : 
                d_curr;
        }

        public static bool Shared(double will, TheMind mind)
        {
            double awareness = 0.0d;
            SimpleAgent agent = new SimpleAgent(mind);

            double awareA = 1.0d - awareness;
            double awareB = awareness;
            double zero = 0.0.Norm1(mind);
            double w_agentA = will.Norm1(mind);
            double w_agentB = agent.SimulateDeltaVelocity();
            
            double w_shared = (awareA * w_agentA + awareB * w_agentB);
            bool down = w_shared <= zero;

            bool flip = mind.prob.Use(w_shared * 100.0d, down, mind);

            agent.SetProperty(flip);

            return flip;
        }

        public static bool Probability(double will, TheMind mind)
        {
            bool down = will <= 0;

            return mind.prob.Use(will, down, mind);
        }        

        //public HARDDOWN ToHard()
        //{
        //    return WillPropNorm0 <= 0.0d ?
        //        HARDDOWN.YES :
        //        HARDDOWN.NO;
        //}

        //public FUZZYDOWN ToFuzzy()
        //{
        //    switch (WillPropNorm100)
        //    {
        //        case <= 20.0d: return FUZZYDOWN.VERYYES;
        //        case <= 40.0d: return FUZZYDOWN.YES;
        //        case <= 60.0d: return FUZZYDOWN.MAYBE;
        //        case <= 80.0d: return FUZZYDOWN.NO;
        //        case <= 100.0d: return FUZZYDOWN.VERYNO;
        //        default: throw new NotSupportedException("ToFuzzy");
        //    }
        //}

        //public PERIODDOWN ToPeriod()
        //{
        //    return Count(HARDDOWN.YES) > Count(HARDDOWN.NO) ?
        //        PERIODDOWN.YES :
        //        PERIODDOWN.NO;
        //}
    }
}
