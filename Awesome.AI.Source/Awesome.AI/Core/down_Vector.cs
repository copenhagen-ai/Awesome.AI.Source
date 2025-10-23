//using Awesome.AI.Common;
//using Awesome.AI.Core;
//using Awesome.AI.CoreSystems;
//using Awesome.AI.Variables;
//using static Awesome.AI.Variables.Enums;

//namespace Awesome.AI.Awesome.AI.Core
//{
//    public class down_Vector
//    {
//        public double Direction
//        {
//            get
//            {
//                //bool val = Vector.xx <= 0.0d;
//                //return val ? -1.0d : 1.0d;
                
//                double d_curr = mind.mech_current.mp.d_curr;
//                return d_curr <= 0.0d ? -1.0d : 1.0d;
//            }
//        }

//        public bool IsYes
//        {
//            get
//            {
//                bool val = Vector.xx <= 0.0d;
                
//                return val;
//            }
//        }

//        public bool IsNo
//        {
//            get
//            {
//                bool val = Vector.xx > 0.0d;
                
//                return val;
//            }
//        }

//        public double Norm
//        {
//            get
//            {
//                double xx = Vector.xx;

//                return mind.calc.Normalize(xx, -1.0d, 1.0d, 0.0d, 100.0d);
//            }
//        }

//        public int Error { get; set; }
//        public List<double> Ratio { get; set; }
//        private List<bool> Errors {  get; set; }
//        private MyVector2D Vector { get; set; }

//        private TheMind mind;
//        private down_Vector() { }
//        public down_Vector(TheMind mind)
//        {
//            this.mind = mind;
//            Vector = new MyVector2D(0.0d, 1.0d, 1.0d, 0.0d);
//            Ratio = new List<double>();
//            Errors = new List<bool>();
//        }

//        public void SetYES()
//        {
//            Vector = new MyVector2D(-1.0d, 0.0d, 1.0d, null);
//        }

//        public void SetNO()
//        {
//            Vector = new MyVector2D(1.0d, 0.0d, 1.0d, null);
//        }

//        public void SetXX(double norm)
//        {
//            Vector = new MyVector2D(norm, 0.0d, 1.0d, null);
//        }


//        public void Update()
//        {
//            if (mind.z_current != "z_noise")
//                return;

//            //code: before or after?
//            Ratio.Add(Direction);
//            if (Ratio.Count > CONST.LAPSES)
//                Ratio.RemoveAt(0);

//            //Discrete();

//            Continous();
//        }

//        public int Count(HARDDOWN dir)
//        {
//            int count = 0;
//            switch (dir)
//            {
//                case HARDDOWN.YES: count = Ratio.Where(z => z <= 0.0d).Count(); break;
//                case HARDDOWN.NO: count = Ratio.Where(z => z > 0.0d).Count(); break;
//            }

//            return count;
//        }

//        public void SetError(bool err)
//        {
//            Errors.Add(err);
//            if(Errors.Count > 100)
//                Errors.RemoveAt(0);

//            Error = Errors.Count(x => x == true);
//        }        

//        public void Discrete()
//        {
//            /*
//             * NO is to say no to going downwards
//             * */

//            SimpleAgent agent = new SimpleAgent(mind);

//            double d_curr = mind.mech_current.mp.d_curr;

//            bool down1 = d_curr <= 0.0d;
//            bool down2 = agent.SimulateDirection() <= 0.0d;
//            bool save = down1;

//            if (CONST.Logic == LOGICTYPE.CLASSICAL) //this a logic error..
//                down1 = down1.TheHack(mind);

//            if (CONST.Logic == LOGICTYPE.PROBABILITY)
//                down1 = down1.Probability(mind);

//            if (CONST.Logic == LOGICTYPE.QUBIT)
//                down1 = down1.Qubit(down2, mind);

//            SetError(save != down1);

//            if (down1)
//                SetYES();
//            else
//                SetNO();
//        }

//        public void Continous()
//        {
//            SimpleAgent agent = new SimpleAgent(mind);

//            double d_curr = mind.mech_current.mp.d_curr;
//            double d_norm = mind.mech_current.mp.d_100;
//            double d_save = mind.mech_current.mp.d_100;

//            bool down1 = d_curr <= 0.0d;
//            bool down2 = agent.SimulateDirection() <= 0.0d;

//            d_norm = mind.calc.Normalize(d_norm, 0.0d, 100.0d, -1.0d, 1.0d);
//            d_save = mind.calc.Normalize(d_save, 0.0d, 100.0d, -1.0d, 1.0d);

//            if (CONST.Logic == LOGICTYPE.CLASSICAL)
//                throw new NotImplementedException("Down, Continous");

//            if (CONST.Logic == LOGICTYPE.PROBABILITY && down1.Probability(mind))
//                d_norm = d_norm * -1.0d;

//            if (CONST.Logic == LOGICTYPE.QUBIT && down1.Qubit(down2, mind))
//                d_norm = d_norm * -1.0d;

//            SetError(d_save != d_norm);

//            SetXX(d_norm);
//        }

//        public HARDDOWN ToHard()
//        {
//            return IsYes ? 
//                HARDDOWN.YES : 
//                HARDDOWN.NO;
//        }

//        public FUZZYDOWN ToFuzzy()
//        {
//            switch (Norm)
//            {
//                case <= 20.0d: return FUZZYDOWN.VERYYES; 
//                case <= 40.0d: return FUZZYDOWN.YES; 
//                case <= 60.0d: return FUZZYDOWN.MAYBE; 
//                case <= 80.0d: return FUZZYDOWN.NO; 
//                case <= 100.0d: return FUZZYDOWN.VERYNO; 
//                default: throw new NotSupportedException("ToFuzzy");
//            }
//        }

//        public PERIODDOWN ToPeriod()
//        {
//            return Count(HARDDOWN.YES) > Count(HARDDOWN.NO) ?
//                PERIODDOWN.YES :
//                PERIODDOWN.NO;
//        }
//    }
//}
