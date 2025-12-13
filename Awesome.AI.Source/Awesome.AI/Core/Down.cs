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
            get => WillCurr <= 0.0d ? -1.0d : 1.0d;
        }

        public double WillCurr { get; set; }
        public double WillNorm { get; set; }
        public double WillProp { get; set; }//zero
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

            WillProp = 1.0d;
        }

        public void Update()
        {
            if (mind.z_current != "z_noise")
                return;

            //Discrete();
            Continous();

            //code: before or after?
            Ratio.Add(WillCurr);
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

        public void SetYES()
        {
            throw new NotImplementedException();
        }

        public void SetNO()
        {
            throw new NotImplementedException();    
        }

        [Obsolete]
        public void Discrete()
        {
            /*
             * NO is to say no to going downwards
             * */

            double d_curr = mind.mech_current.mp.d_curr;

            bool down = d_curr <= 0.0d;
            bool save = down;

            if (CONST.Logic == LOGICTYPE.PROBABILITY)
                down = Probability(down, mind);

            if (CONST.Logic == LOGICTYPE.QUBIT)
                down = Qubit(mind);

            SetError(save != down);

            if (down)
                SetYES();
            else
                SetNO();
        }

        public void Continous()
        {
            /*
             * this can be extended with a mechanism to alter between the two
             * thereby expressing levels of social entanglement
             * */

            double d_curr = mind.mech_current.mp.d_curr;
            double d_zero = mind.mech_current.mp.d_100.Zero(mind);
            double d_save = mind.mech_current.mp.d_100.Zero(mind);

            bool down = d_curr <= 0;
            
            if (CONST.Logic == LOGICTYPE.PROBABILITY && Probability(down, mind) && !Inertia())
                d_zero *= -1.0d;

            if (CONST.Logic == LOGICTYPE.QUBIT && Qubit(mind) && !Inertia())
                d_zero *= -1.0d;

            bool flip = d_save != d_zero;
            DoFlip(flip, d_curr, out d_curr);
            SetError(flip);

            WillProp = d_zero;
            WillNorm = d_zero.Norm(mind);
            WillCurr = d_curr;

            if (double.IsNaN(WillProp))
                throw new Exception("NAN");
        }

        private bool Inertia()
        {
            /*
             * very simple, should be multistep
             * cancels out some of the direction changes
             * */

            //return false;

            double p_curr = mind.mech_current.mp.p_curr;
            double p_prev = mind.mech_current.mp.p_prev;

            bool p_up = p_curr > p_prev;
            bool d_up = Dir > 0.0d;

            return (p_up && d_up) || (!p_up && !d_up);
        }

        private void DoFlip(bool flip, double d_curr, out double _out)
        {
            double low = mind.mech_current.mp.d_out_low;
            double high = mind.mech_current.mp.d_out_high;

            _out = flip ? 
                d_curr.Flip(low, high, mind) : 
                d_curr;
        }

        public static bool Probability(bool _b, TheMind mind)
        {
            return mind.prob.Use(_b, mind);
        }

        public static bool Qubit(TheMind mind)
        {
            /*
             * proof of concept
             * */

            int measure = mind.quantum.Run();

            SimpleAgent agent = new SimpleAgent(mind);

            agent.SetProperty(measure > 0);

            return measure > 0;
        }

        public HARDDOWN ToHard()
        {
            return WillCurr <= 0.0d ?
                HARDDOWN.YES :
                HARDDOWN.NO;
        }

        public FUZZYDOWN ToFuzzy()
        {
            switch (WillNorm)
            {
                case <= 20.0d: return FUZZYDOWN.VERYYES;
                case <= 40.0d: return FUZZYDOWN.YES;
                case <= 60.0d: return FUZZYDOWN.MAYBE;
                case <= 80.0d: return FUZZYDOWN.NO;
                case <= 100.0d: return FUZZYDOWN.VERYNO;
                default: throw new NotSupportedException("ToFuzzy");
            }
        }

        public PERIODDOWN ToPeriod()
        {
            return Count(HARDDOWN.YES) > Count(HARDDOWN.NO) ?
                PERIODDOWN.YES :
                PERIODDOWN.NO;
        }
    }
}
