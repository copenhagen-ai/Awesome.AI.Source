using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Core.Internals;
using Awesome.AI.CoreSystems;
using Awesome.AI.Source.Awesome.AI.Common;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Source.Awesome.AI.Core.Internals
{
    public class Operators
    {
        public BaseProperties prop {  get; set; }
        public List<double> _ratio { get; set; }
        public List<bool> _errors { get; set; }
        public bool RES_BOOL { get; set; }
        public bool RES_DOUBLE { get; set; }
        public int _error { get; set; }

        public TheMind mind;
        private Operators() { }
        public Operators(TheMind mind)
        {
            this.mind = mind;
            this.prop = mind.mech.mp.eprops;

            _ratio = new List<double>();
            _errors = new List<bool>();
        }
        
        public virtual object Output(object obj) { return null; }
        public virtual void Modify() { }
    }

    public class Down : Operators
    {
        public Down(TheMind mind) : base(mind)
        {            
        }

        public override object Output(object vec)
        {
            if (RES_BOOL)
                return ((GPTVector2D)vec).Unit().ReverseUnit();

            return ((GPTVector2D)vec).Unit();
        }

        public override void Modify()
        {
            SetDown();

            double d_curr = prop.Conflict();
            double d_zero = d_curr.Norm0DV(mind);
            double d_save = d_curr.Norm0DV(mind);
            
            if (mind.bot.logic == LOGICTYPE.PROBABILITY && Probability(d_curr, mind)/* && NoInertia() && NoMomentum()*/)
                d_zero *= -1.0d;

            if (mind.bot.logic == LOGICTYPE.SHARED && Shared(d_curr, mind)/* && NoInertia()*/ && NoMomentum())
                d_zero *= -1.0d;

            bool flip = d_save != d_zero;

            RES_BOOL = flip ? !RES_BOOL : RES_BOOL;
            
            SetError(flip);
            SetRatio(RES_BOOL);
        }

        public void SetDown()
        {
            double curr_dir = mind.mech.mp.eprops.Conflict() < 0 ? -1.0d : 1.0d;

            RES_BOOL = curr_dir == -1.0d;            
        }

        public int Count(HARDDOWN dir)
        {
            int count = 0;
            switch (dir)
            {
                case HARDDOWN.YES: count = _ratio.Where(z => z <= 0.0d).Count(); break;
                case HARDDOWN.NO: count = _ratio.Where(z => z > 0.0d).Count(); break;
            }

            return count;
        }

        public void SetRatio(bool down)
        {
            double ratio = down ? -1d : 1d;

            _ratio.Add(ratio);
            if (_ratio.Count > CONST.LAPSES)
                _ratio.RemoveAt(0);
        }

        public void SetError(bool err)
        {
            _errors.Add(err);
            if (_errors.Count > 100)
                _errors.RemoveAt(0);

            _error = _errors.Count(x => x == true);
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
             * cancels out some of the direction changes
             * */

            double mom = mind.mech.ms.mom_sym_curr;

            double abs = Math.Abs(mom);

            if(abs > abs_max)
                abs_max = abs;

            bool res = abs < abs_max * 0.1d;

            return res;
        }

        public static bool Shared(double _d, TheMind mind)
        {
            double awareness = 0.0d;
            SimpleAgent agent = new SimpleAgent(mind);

            double awareA = 1.0d - awareness;
            double awareB = awareness;
            double zero = 0.0.Norm1VV(mind);
            double w_agentA = _d.Norm1VV(mind);
            double w_agentB = agent.SimulateDeltaVelocity();
            
            double w_shared = awareA * w_agentA + awareB * w_agentB;
            bool down = w_shared <= zero;
            bool flip = mind.prob.Use(w_shared * 100.0d, down, mind);

            agent.SetProperty(flip);

            return flip;
        }

        public static bool Probability(double _d, TheMind mind)
        {
            bool down = _d <= 0;

            return mind.prob.Use(_d, down, mind);
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
