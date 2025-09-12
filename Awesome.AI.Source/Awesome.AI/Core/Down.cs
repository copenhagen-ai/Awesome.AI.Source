using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.CoreInternals;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Awesome.AI.Core
{
    public class Down
    {
        public double Direction { get { return Vector.xx; } }
        public FUZZYDOWN Fuzzy { get; set; }
        public PERIODDOWN Period { get; set; }
        public List<double> Ratio { get; set; }

        MyVector2D Vector { get; set; }

        private TheMind mind;
        private Down() { }
        public Down(TheMind mind)
        {
            this.mind = mind;
            Vector = new MyVector2D(1.0d, 0.0d, 1.0d, 0.0d);
            Ratio = new List<double>();
        }

        //public void Flip()
        //{
        //    /*
        //     * we change direction 
        //     * */

        //    if(IsYes())
        //        SetNO();

        //    if(IsNo())
        //        SetYES();
        //}

        public void SetYES()
        {
            Vector = new MyVector2D(-1.0d, 0.0d, 1.0d, null);
        }

        public void SetNO()
        {
            Vector = new MyVector2D(1.0d, 0.0d, 1.0d, null);
        }

        public bool IsYes()
        {
            return Vector.xx < 0.0d;
        }

        public bool IsNo()
        {
            return Vector.xx >= 0.0d;
        }

        public double NormX() 
        {
            double x = Vector.xx;
            double res = mind.calc.Normalize(x, -1.0d, 1.0d, 0.0d, 100.0d);

            return res;
        }
        
        public void Update()
        {
            if (mind.z_current != "z_noise")
                return;

            Ratio.Add(Vector.xx);

            if (Ratio.Count > CONST.LAPSES)
                Ratio.RemoveAt(0);


            ToHard();

            ToFuzzy();

            ToPeriod();
        }

        public int Count(bool is_yes)
        {
            int count = 0;
            switch (is_yes)
            {
                case true: count = Ratio.Where(z => z < 0.0d).Count(); break;
                case false: count = Ratio.Where(z => z >= 0.0d).Count(); break;
            }

            return count;
        }

        public void ToHard()
        {
            /*
             * NO is to say no to going downwards
             * name is because it used to be a boolean
             * */

            SimpleAgent agent = new SimpleAgent(mind);

            //double d_momentum = mind.mech_current.d_prev;
            double d_momentum = mind.mech_current.d_curr;
                                    
            bool down1 = d_momentum <= 0.0d;
            bool down2 = agent.SimulateDown();

            if (CONST.Logic == LOGICTYPE.CLASSICAL) //is this a logic error?
                down1 = !down1;

            if (CONST.Logic == LOGICTYPE.PROBABILITY)
                down1 = mind.prob.Use(down1, mind);

            if (CONST.Logic == LOGICTYPE.QUBIT)
                down1 = mind.quantum.usage.DoQuantumXOR(down1, down2);

            if (down1) 
                SetYES();
            else 
                SetNO();
        }

        public void ToFuzzy()
        {
            double norm = mind.mech_current.p_100;

            switch (norm)
            {
                case <= 20.0d: Fuzzy = FUZZYDOWN.VERYYES; break;
                case <= 40.0d: Fuzzy = FUZZYDOWN.YES; break;
                case <= 60.0d: Fuzzy = FUZZYDOWN.MAYBE; break;
                case <= 80.0d: Fuzzy = FUZZYDOWN.NO; break;
                case <= 100.0d: Fuzzy = FUZZYDOWN.VERYNO; break;
                default: throw new NotSupportedException("ToFuzzy");
            }
        }

        public void ToPeriod()
        {
            /*
             * indifferent of the direction
             * */

            int count_no = Ratio.Count(x => x >= 0.0d);
            int count_yes = Ratio.Count(x => x < 0.0d);

            Period = count_no >= count_yes ?
                PERIODDOWN.NO :
                PERIODDOWN.YES;          
        }
    }
}
