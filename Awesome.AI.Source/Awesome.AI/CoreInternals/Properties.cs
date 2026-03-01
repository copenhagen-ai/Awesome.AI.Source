using Awesome.AI.Common;
using Awesome.AI.Core;
using Awesome.AI.Variables;
using static Awesome.AI.Variables.Enums;

namespace Awesome.AI.Awesome.AI.Core
{
    public class Properties
    {
        public class MyModifiers
        {
            public double Mod(double _base, double value, string prop, Dictionary<string, double> mods)
            {
                if (prop == "base")
                    _base = 1.0d;
                else
                {
                    foreach (var mod in mods)
                    {
                        if (mod.Key == "base")
                            continue;

                        if (mod.Key == prop)
                            _base *= mod.Value;
                    }
                }
                
                return _base * value;
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

            public double Run(double val, string key2)
            {
                double res = 1.0d;

                foreach(var v in _data)
                    res *= this[v.Key.Item1, key2];

                return res * val;
            }
        }

        private double dv_prev { get; set; }
        public double Dir(string ax)
        {
            if (ax == "will")
                ax = "base";

            double dv = mind.mech_high.mp.props.PropsOut[ax];
            double dir = dv > dv_prev ? 1.0d : -1.0d;

            return dir;            
        }

        //private List<string> Attributes { get; set; }
        private MyModifiers Mods{ get; set; }
        private MyMatrix Matrix { get; set; }
        public Dictionary<string, double> PropsIn { get; set; }
        public Dictionary<string, double> PropsOut { get; set; }
        public double Base {  get; set; }

        private TheMind mind;
        private Properties() { }
        public Properties(TheMind mind, PROPS props)
        {
            this.mind = mind;

            GetProps(props, out double _base, out Dictionary<string, double> attr, out Dictionary<(string, string), double> mat);
            
            //int count = a.Length;

            PropsOut = new Dictionary<string, double>();
            PropsIn = attr;
            Base = _base;
            Mods = new MyModifiers();
            Matrix = new MyMatrix();
            
            foreach (var kv in attr) 
                PropsOut.Add(kv.Key, 0.0d);
                        
            foreach (var kv in mat)
                Matrix[kv.Key.Item1, kv.Key.Item2] = kv.Value;            
        }

        public void Update()
        {
            if (mind.z_current != "z_mech")
                return;

            var _norm = mind.mech_high.ms.dv_sym_100.Norm0(mind, 0.0d, 100.0d);

            foreach (var kv in PropsIn)
            {
                string prop = kv.Key;
                double _base = Base;
                double res = 0.0d;

                res = Mods.Mod(_base, _norm, prop, PropsIn);
                res = Matrix.Run(res, prop);
                PropsOut[prop] = res;                        
            }
        }

        private void GetProps(PROPS props, out double _base, out Dictionary<string, double> attr, out Dictionary<(string, string), double> m)
        {
            switch (props)
            {
                case PROPS.BRAINWAVE:
                    _base = 1.0d;

                    //attention, readyness
                    attr = new Dictionary<string, double> { { "base", double.NaN }, { CONST.axis_2_brain, 2.0 }, { CONST.axis_3_brain, 1.2 } };

                    m = new Dictionary<(string, string), double>();
                    m.Add(("base", CONST.axis_3_brain), 1.0d);
                    m.Add((CONST.axis_2_brain, CONST.axis_3_brain), 1.0d);
                    break;

                case PROPS.COMMUNICATION:
                    _base = 1.0d;

                    //opinion, temporality, abstraction
                    attr = new Dictionary<string, double> { { "base", double.NaN }, { CONST.axis_2_comm, 2.0 }, { CONST.axis_3_comm, 0.5 }, { CONST.axis_4_comm, 1.2 } };

                    m = new Dictionary<(string, string), double>();
                    m.Add(("base", CONST.axis_3_comm), 0.65d);
                    m.Add((CONST.axis_2_comm, CONST.axis_3_comm), 0.45d);
                    m.Add((CONST.axis_4_comm, CONST.axis_2_comm), 0.35d);

                    break;

                default: throw new NotImplementedException("Properties, GetProps");
            }
        }
    }
}
