using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.CoreInternals;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Awesome.AI.Core
{
    public class down_Property
    {
        public class MyModifiers
        {
            public double Mod_A(double value, string prop)
            {
                double _base = -0.05;

                if (prop == "opinion")//stronger damping for opinion
                    _base *= 2.0;

                if (prop == "temporality")//stronger damping for temporality
                    _base *= 2.0;

                if (prop == "abstraction")//stronger damping for abstraction
                    _base *= 1.2;

                return _base * value;
            }

            public double Mod_B(double value, string prop)
            {
                double _base = -0.5;

                if (prop == "opinion")//stronger damping for opinion
                    _base = 1.0;

                if (prop == "temporality")//stronger damping for temporality
                    _base *= 1.5;

                if (prop == "abstraction")//stronger damping for abstraction
                    _base *= 0.5;

                return _base * value;
            }

            public double Run(double value, string prop)
            {
                value = Mod_A(value, prop);
                value = Mod_B(value, prop);

                return value;
            }
        }

        public class MyMatrix
        {
            private Dictionary<(string, string), double> _data = new();

            public double this[string key1, string key2]
            {
                get
                {
                    if (_data.TryGetValue((key1, key2), out var value))
                        return value;

                    return 1.0;
                }
                set => _data[(key1, key2)] = value;
            }

            public double Run(string key2)
            {
                double res = 0.0d;

                foreach(var v in _data)
                    res *= this[v.Key.Item1, key2];

                return res;
            }
        }

        public double Direction
        {
            get
            {
                //bool val = Axis[Current] <= 0.0d;
                //return val ? -1.0d : 1.0d;
                
                double d_curr = mind.mech_current.mp.d_curr;
                return d_curr <= 0.0d ? -1.0d : 1.0d;
            }
        }

        public bool IsYes
        {
            get
            {
                bool val = Axis[Current] <= 0.0d;
                
                return val;
            }
        }

        public bool IsNo
        {
            get
            {
                bool val = Axis[Current] > 0.0d;
                
                return val;
            }
        }

        public double Norm100
        {
            get
            {
                double xx = Axis[Current];

                return mind.calc.Normalize(xx, -1.0d, 1.0d, 0.0d, 100.0d);
            }
        }

        public double NormZero
        {
            get
            {
                double xx = Axis[Current];

                return xx;
            }
        }

        private MyModifiers Mods{ get; set; }
        private MyMatrix Matrix { get; set; }
        public int Error { get; set; }
        public string Current { get; set; }
        public List<double> Ratio { get; set; }
        private List<bool> Errors {  get; set; }
        private List<string> Properties { get; set; }
        private Dictionary<string, double> Axis { get; set; }

        private TheMind mind;
        private down_Property() { }
        public down_Property(TheMind mind)
        {
            this.mind = mind;

            Ratio = new List<double>();
            Errors = new List<bool>();

            Properties = new List<string> { "noise", "opinion", "temporality", "abstraction" };
            Axis = new Dictionary<string, double>();

            foreach (var prop in Properties)
                Axis.Add(prop, 1.0d);

            Mods = new MyModifiers();

            Matrix = new MyMatrix();
            Matrix["noise", "temporality"] = 0.65d;
            Matrix["opinion", "temporality"] = 0.45d;
            Matrix["abstraction", "opinion"] = 0.35d;
        }

        public void SetYES()
        {
            Axis[Current] = -1.0d;
        }

        public void SetNO()
        {
            Axis[Current] = 1.0d;
        }

        public void SetXX(double norm)
        {
            if (norm > 1.0d)
                norm = 1.0d;

            if (norm < -1.0d)
                norm = -1.0d;

            Axis[Current] = norm;
        }


        public void Update()
        {
            if (mind.z_current != "z_noise")
                return;

            foreach (var prop in Properties)
            {
                Current = prop;

                //Discrete(prop);
                Continous(prop);

                if (prop != "noise")
                    continue;
                
                //code: before or after?
                Ratio.Add(Direction);
                if (Ratio.Count > CONST.LAPSES)
                    Ratio.RemoveAt(0);
            }
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
            if(Errors.Count > 100)
                Errors.RemoveAt(0);

            Error = Errors.Count(x => x == true);
        }        

        public void Discrete(string prop)
        {
            /*
             * NO is to say no to going downwards
             * */

            SimpleAgent agent = new SimpleAgent(mind);

            double d_curr = mind.mech_current.mp.d_curr;

            bool down1 = d_curr <= 0.0d;
            bool down2 = agent.SimulateDirection() <= 0.0d;
            bool save = down1;

            if (CONST.Logic == LOGICTYPE.CLASSICAL) //this a logic error..
                down1 = down1.TheHack(mind);

            if (CONST.Logic == LOGICTYPE.PROBABILITY)
                down1 = down1.Probability(mind);

            if (CONST.Logic == LOGICTYPE.QUBIT)
                down1 = down1.Qubit(down2, mind);

            if (prop == "noise")
                SetError(save != down1);

            if (down1)
                SetYES();
            else
                SetNO();
        }

        public void Continous(string prop)
        {
            SimpleAgent agent = new SimpleAgent(mind);

            double d_curr = mind.mech_current.mp.d_curr;
            double d_norm = mind.mech_current.mp.d_100;
            double d_save = mind.mech_current.mp.d_100;

            bool down1 = d_curr <= 0.0d;
            bool down2 = agent.SimulateDirection() <= 0.0d;

            d_norm = mind.calc.Normalize(d_norm, 0.0d, 100.0d, -1.0d, 1.0d);
            d_save = mind.calc.Normalize(d_save, 0.0d, 100.0d, -1.0d, 1.0d);

            if (CONST.Logic == LOGICTYPE.CLASSICAL)
                throw new NotImplementedException("Down, Continous");

            if (CONST.Logic == LOGICTYPE.PROBABILITY && down1.Probability(mind))
                d_norm = d_norm * -1.0d;

            if (CONST.Logic == LOGICTYPE.QUBIT && down1.Qubit(down2, mind))
                d_norm = d_norm * -1.0d;

            if (prop == "noise")
                SetError(d_save != d_norm);

            d_norm = Mods.Run(d_norm, prop);
            d_norm = Matrix.Run(prop);

            SetXX(d_norm);
        }

        public HARDDOWN ToHard()
        {
            return IsYes ? 
                HARDDOWN.YES : 
                HARDDOWN.NO;
        }

        public FUZZYDOWN ToFuzzy()
        {
            switch (Norm100)
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
